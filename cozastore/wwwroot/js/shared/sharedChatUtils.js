// =======================================================
// === 1. دالة تهيئة الوقت (Format Time) ===
// =======================================================
function formatMessageTime(isoTimestamp) {
    if (!isoTimestamp) return '';
    const date = new Date(isoTimestamp);
    if (isNaN(date.getTime())) {
        console.error('Failed to parse date:', isoTimestamp);
        return 'Invalid Time';
    }
    // عرض الوقت بتنسيق سهل القراءة (مثلاً: 02:18 PM)
    return date.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
}


// =======================================================
// === 2. دالة بناء عنصر الرسالة (Build Message Element) ===
// =======================================================
function buildMessageElement(messageData, currentUserId) {
    // 1. تحديد الخصائص
    const isSender = messageData.senderId === currentUserId;
    const messageAlignment = isSender ? "ms-auto" : "me-auto";
    const messageBackground = isSender ? "bg-primary text-white" : "bg-light";
    console.log('Message Content:', messageData.content ? messageData.content.substring(0, 30) + '...' : 'File Message');
    console.log('Message Sender ID Clint:', messageData.senderId);
    console.log('Current Viewer ID (The one watching):', currentUserId);
    const textColor = isSender ? "text-white" : "text-dark";
    const smallTextColor = isSender ? "text-white-50" : "text-muted";
    const formattedTime = formatMessageTime(messageData.timestamp); // ⬅️ استخدام الدالة المدمجة

    const containerDiv = document.createElement('div');
    containerDiv.className = `d-flex mb-3 ${messageAlignment}`;
    containerDiv.style.maxWidth = '80%';

    const cardDiv = document.createElement('div');
    cardDiv.className = `card ${messageBackground} border-0 shadow-sm`;
    cardDiv.style.borderRadius = '1rem';

    let contentHtml = '';

    // 2. محتوى الملف
    if (messageData.isFile && messageData.filePath) {
        const fileName = messageData.filePath.substring(messageData.filePath.lastIndexOf('/') + 1);
        let filePreview = '';
        if (fileName.match(/\.(jpeg|jpg|gif|png)$/i)) {
            filePreview = `<a href="${messageData.filePath}" target="_blank">
                              <img src="${messageData.filePath}" alt="ملف مرفق" class="img-fluid rounded mb-1" style="max-height: 150px; cursor: pointer;" />
                            </a>`;
        }

        contentHtml += `
            <div class="d-flex flex-column mb-1">
                ${filePreview}
                <a href="${messageData.filePath}" target="_blank" class="text-decoration-none ${textColor} fw-bold">
                    <i class="fas fa-file-download me-1"></i> ${fileName}
                </a>
            </div>
        `;
    }

    // 3. محتوى النص
    if (messageData.content) {
        contentHtml += `<p class="card-text mb-0">${messageData.content}</p>`;
    }

    // 4. بناء جسم الرسالة والوقت
    cardDiv.innerHTML = `
        <div class="card-body p-2">${contentHtml}</div>
        <div class="card-footer p-1 border-0" style="background: transparent;">
            <small class="float-end ${smallTextColor}" style="font-size: 0.7rem;">
                ${formattedTime}
            </small>
        </div>
    `;

    containerDiv.appendChild(cardDiv);
    return containerDiv;
}


// =======================================================
// === 3. دالة التمرير لأسفل (Scroll To Bottom) ===
// =======================================================
function scrollToBottom() {
    const messagesList = document.getElementById('messagesList');
    if (messagesList) {
        // إضافة تأخير بسيط لضمان انتهاء تحديث الـ DOM
        setTimeout(() => {
            messagesList.scrollTop = messagesList.scrollHeight;
        }, 50);
    }
}