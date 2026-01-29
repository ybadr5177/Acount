// =======================================================
// === 0. دالة تهيئة الوقت (Format Time) ===
// 🚨 تم حذف هذه الدالة (formatMessageTime) لأنها أصبحت موجودة في sharedChatUtils.js
// =======================================================


// =======================================================
// === 1. دالة لقراءة الكوكي ===
// =======================================================
function getCookie(name) {
    const value = `; ${document.cookie}`;
    const parts = value.split(`; ${name}=`);
    if (parts.length === 2) return parts.pop().split(';').shift();
}

// =======================================================
// === 2. تعريف المتغيرات ===
// =======================================================
const SENDER_ID = document.getElementById('senderId')?.value;
const CONVERSATION_ID = document.getElementById('conversationId')?.value;
const messageInput = document.getElementById('messageInput');
const fileInput = document.getElementById('fileInput');
const sendMessageButton = document.getElementById('sendMessageButton');
const messagesList = document.getElementById('messagesList');

// 🚨 التعديل: استخدام المنفذ الصحيح أو مسار نسبي. تم تثبيت العنوان الآن على أساس الافتراض.
const HUB_URL = "https://localhost:7056/chathub";

const connection = new signalR.HubConnectionBuilder()
    .withUrl(HUB_URL, {
        accessTokenFactory: () => {
            const token = getCookie('AuthToken');
            return token;
        }
    })
    .withAutomaticReconnect()
    .build();

// =======================================================
// === 3. منطق الاتصال والاستقبال ===
// =======================================================
// استقبال الرسائل
connection.on("ReceiveMessage", function (messageData) {
    // 💡 الآن: buildMessageElement تستخدم دالة الوقت من sharedChatUtils.js
    if (messageData.conversationId == CONVERSATION_ID) {
        const messageElement = buildMessageElement(messageData, SENDER_ID);
        messagesList.appendChild(messageElement);
        scrollToBottom();
    }
});

// =======================================================
// === 4. دالة رفع الملفات عبر AJAX ===
// =======================================================
async function uploadFileToController(file) {
    const currentToken = getCookie('AuthToken');
    if (!currentToken) throw new Error("التوكن غير موجود للوصول إلى الخادم.");

    const formData = new FormData();
    formData.append('file', file);
    const uploadUrl = '/Chat/UploadFile';

    const response = await fetch(uploadUrl, {
        method: 'POST',
        headers: { 'Authorization': `Bearer ${currentToken}` },
        body: formData
    });

    if (!response.ok) {
        const error = await response.json();
        throw new Error(error.message || 'فشل في رفع الملف على الخادم.');
    }
    const data = await response.json();
    return data.fileUrl;
}

// =======================================================
// === 5. إدارة عملية الإرسال (مع التحديث التفاؤلي وتصحيح الوقت) ===
// =======================================================
sendMessageButton?.addEventListener('click', handleSendMessage);
messageInput?.addEventListener('keypress', function (e) {
    if (e.key === 'Enter') handleSendMessage(e);
});

async function handleSendMessage(event) {
    event.preventDefault();
    const textContent = messageInput.value.trim();
    const file = fileInput.files[0];

    if (!textContent && !file) return;

    sendMessageButton.disabled = true;
    let filePath = "";
    let isFile = false;

    try {
        if (file) {
            isFile = true;
            filePath = await uploadFileToController(file);
        }

        if (textContent || filePath) {
            if (connection.state !== signalR.HubConnectionState.Connected) {
                throw new Error("الاتصال بمركز الدردشة غير متاح. يرجى تسجيل الدخول مجدداً.");
            }

            // 🚨 الخطوة 1: التحديث الفوري (Optimistic Update)
            const optimisticMessage = {
                senderId: SENDER_ID,
                content: textContent,
                isFile: isFile,
                filePath: filePath,
                // 💡 الإصلاح: نستخدم toISOString() هنا للتنسيق القياسي
                timestamp: new Date().toISOString(),
                conversationId: CONVERSATION_ID
            };
            const messageElement = buildMessageElement(optimisticMessage, SENDER_ID);
            messagesList.appendChild(messageElement);
            scrollToBottom();

            messageInput.value = "";
            fileInput.value = "";

            // 🚨 الخطوة 2: إرسال البيانات عبر SignalR
            await connection.invoke("SendMessage",
                SENDER_ID,
                "Admin", // RecipientID: نرسل إلى مجموعة المدراء
                textContent,
                isFile,
                filePath
            );
        }
    } catch (err) {
        console.error("خطأ أثناء الإرسال: ", err.toString());
        console.error("فشل في الإرسال: " + (err.message || err.toString()));
    } finally {
        sendMessageButton.disabled = false;
    }
}

// =======================================================
// === 6. بدء الاتصال (مع دالة الانضمام للمجموعة) ===
// =======================================================
async function start() {
    try {
        await connection.start();
        console.log("SignalR Connected (Client). UserID:", SENDER_ID);

        // 💡 الخطوة الحاسمة: إخبار الـ Hub بالانضمام إلى مجموعة العميل
        if (SENDER_ID) {
            await connection.invoke("JoinClientGroup", SENDER_ID);
            console.log(`Client joined group: Client_${SENDER_ID}`);
        } else {
            console.warn("SENDER_ID غير متوفر. لم يتم الانضمام إلى مجموعة العميل.");
        }

        // 💡 التأكد من التمرير لأسفل عند تحميل الصفحة
        scrollToBottom();

    } catch (err) {
        console.error("SignalR Connection Error (Client): ", err);
        setTimeout(start, 5000);
    }
}
start();