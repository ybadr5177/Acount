// adminChat.js (الكود النهائي المُعتمد على User ID كـ Access Token)

// =======================================================
// === 0. دالة مساعدة لتهيئة الوقت (حل مشكلة Invalid Date) ===
// =======================================================
// هذه الدالة تحول تنسيق ISO 8601 القادم من C# إلى وقت سهل القراءة
function formatMessageTime(isoTimestamp) {
    if (!isoTimestamp) return '';

    // إنشاء كائن التاريخ من سلسلة ISO
    const date = new Date(isoTimestamp);

    // التحقق من صلاحية التاريخ
    if (isNaN(date.getTime())) {
        console.error('Failed to parse date:', isoTimestamp);
        return 'Invalid Time';
    }

    // عرض الوقت بتنسيق سهل القراءة (مثلاً: 02:18 PM)
    const options = { hour: '2-digit', minute: '2-digit', hour12: true };
    return date.toLocaleTimeString('en-US', options);
}

// =======================================================
// === 1. تعريف المتغيرات ===
// =======================================================
// تأكد أن هذا المتغير يحمل ID المدير المُسجل دخوله
const ADMIN_SENDER_ID = document.getElementById('adminSenderId')?.value;
const messagesList = document.getElementById('messagesList');
const adminMessageInputArea = document.getElementById('adminMessageInputArea');
const chatHeader = document.getElementById('currentChatHeader');
const messageInput = document.getElementById('messageInput');
const fileInput = document.getElementById('adminFileInput');
const sendMessageButton = document.getElementById('sendMessageButton');

let CURRENT_CONVERSATION_ID = 0;
let CURRENT_RECIPIENT_ID = null; // ID العميل الذي ندردش معه حالياً

// =======================================================
// === 2. اتصال SignalR ===
// =======================================================
const connection = new signalR.HubConnectionBuilder()
    // استخدام مسار نسبي
    .withUrl("/chathub")
    .withAutomaticReconnect()
    .build();

// =======================================================
// === 3. منطق الاستقبال وقائمة المحادثات ===
// =======================================================
// استقبال الرسائل
connection.on("ReceiveMessage", function (messageData) {

    // 💡 المشكلة 1: التحديث الفوري (يحدث إذا كانت الرسالة لنفس المحادثة النشطة)
    if (messageData.conversationId == CURRENT_CONVERSATION_ID) {
        // يجب أن تكون الدالتان buildMessageElement و scrollToBottom متاحتين 
        // ويجب أن تستخدم buildMessageElement الدالة formatMessageTime
        const messageElement = buildMessageElement(messageData, ADMIN_SENDER_ID);
        messagesList.appendChild(messageElement);
        scrollToBottom();
    }

    // 💡 تحديث قائمة المحادثات (إضافة مؤشر أو تحديث ترتيبها)
    updateConversationListItem(messageData.conversationId);
});

// دالة تحديث قائمة المحادثات (إضافة مؤشر جديد وتحديث الترتيب في المستقبل)
function updateConversationListItem(conversationId) {
    const item = document.querySelector(`[data-conversation-id="${conversationId}"]`);
    if (item && conversationId != CURRENT_CONVERSATION_ID) {
        // إذا لم تكن المحادثة النشطة، أظهر مؤشر الرسالة الجديدة
        item.classList.add('list-group-item-warning');
    }
}

// =======================================================
// === 4. دالة تحميل المحادثة (تم تأمينها) ===
// =======================================================
// دالة تحميل المحادثة (تُستدعى من الـ View)
window.loadConversation = async function (conversationId, clientUserId, element) {
    // 💡 هنا يتم تخزين الـ IDs بشكل صحيح
    CURRENT_CONVERSATION_ID = conversationId;
    CURRENT_RECIPIENT_ID = clientUserId;

    chatHeader.textContent = `محادثة مع العميل: ${clientUserId.substring(0, 8)}...`;
    messagesList.innerHTML = '<p class="text-center text-muted mt-5">جاري التحميل...</p>';
    adminMessageInputArea.style.display = 'block';

    document.querySelectorAll('.list-group-item-action').forEach(el => el.classList.remove('active', 'list-group-item-warning'));
    element.classList.add('active');
    element.classList.remove('list-group-item-warning'); // إزالة مؤشر الرسالة الجديدة

    // ... (بقية منطق تحميل السجل كما هو) ...
    const historyUrl = `/Chat/LoadHistory?conversationId=${conversationId}`;
    try {
        const response = await fetch(historyUrl);
        if (!response.ok) {
            console.error(`Load History AJAX Error: Status ${response.status}`);
            messagesList.innerHTML = `<p class="text-center text-danger mt-5">فشل في تحميل السجل. (خطأ ${response.status})</p>`;
            return;
        }

        const html = await response.text();
        messagesList.innerHTML = html;
        // 💡 scrollToBottom يجب أن تكون موجودة في sharedChatUtils.js
        scrollToBottom();

    } catch (error) {
        messagesList.innerHTML = '<p class="text-center text-danger mt-5">فشل في تحميل السجل.</p>';
        console.error('AJAX Load History Failed:', error);
    }
}

// =======================================================
// === 5. دالة الإرسال (تمت إضافة التحديث الفوري وتصحيح الوقت) ===
// =======================================================
sendMessageButton?.addEventListener('click', handleAdminSendMessage);
messageInput?.addEventListener('keypress', function (e) {
    if (e.key === 'Enter') handleAdminSendMessage(e);
});

async function handleAdminSendMessage(event) {
    event.preventDefault();
    const messageContent = messageInput.value.trim();

    // 💡 التحقق من الحالة والبيانات
    if (CURRENT_CONVERSATION_ID === 0 || !CURRENT_RECIPIENT_ID || messageContent === "") {
        console.warn("فشل الإرسال: المحادثة غير محددة أو الرسالة فارغة.");
        return;
    }

    try {
        if (connection.state !== signalR.HubConnectionState.Connected) {
            throw new Error("الاتصال بالدردشة غير متاح. حاول إعادة تحميل الصفحة.");
        }

        // 🚨 الخطوة 1: تحديث الواجهة فوراً (Optimistic Update)
        const optimisticMessage = {
            senderId: ADMIN_SENDER_ID,
            content: messageContent,
            isFile: false,
            filePath: "",
            // 💡 الإصلاح: نستخدم toISOString() هنا لأن الـ Hub يرسل هذا التنسيق.
            // وهذا يضمن أن formatMessageTime تستطيع قراءته.
            timestamp: new Date().toISOString(),
            conversationId: CURRENT_CONVERSATION_ID
        };
        const messageElement = buildMessageElement(optimisticMessage, ADMIN_SENDER_ID);
        messagesList.appendChild(messageElement);
        scrollToBottom();
        messageInput.value = ""; // تفريغ صندوق الإدخال

        // 🚨 الخطوة 2: إرسال البيانات عبر SignalR
        await connection.invoke("SendMessage",
            ADMIN_SENDER_ID,
            CURRENT_RECIPIENT_ID, // ID العميل
            messageContent,
            false,
            ""
        );

    } catch (error) {
        console.error("Error sending admin message:", error);
    }
}


// =======================================================
// === 6. بدء اتصال SignalR (تم تبسيطه) ===
// =======================================================
async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected (Admin). UserID:", ADMIN_SENDER_ID);

    } catch (err) {
        console.error("SignalR Connection Error (Admin): ", err);
        // محاولة إعادة الاتصال
        setTimeout(start, 5000);
    }
}

// 🚨 استدعاء الدالة هنا
start();