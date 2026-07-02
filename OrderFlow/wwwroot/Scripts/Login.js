/**
 * СИНХРОННО собирает данные из формы и генерирует хэш пароля через CryptoJS.
 */
function getHashes() {
    const username = $("#username").val();
    const passwordValue = $("#password").val();

    if (!passwordValue) {
        throw new Error("Поле 'Пароль' не должно быть пустым.");
    }

    // Вычисляем SHA-256 в одну строчку кода
    const hash = CryptoJS.SHA256(passwordValue);

    // Переводим результат в формат Base64, как требует бэкенд C#
    const base64Hash = hash.toString(CryptoJS.enc.Base64);

    return {
        login: username,
        passwordHash: base64Hash
    };
}

/**
 * Генерирует SHA-256 хэш строки и возвращает его в формате Base64.
 * Использует нативное Web Crypto API браузера.
 */
async function computeSha256(message) {
    if (!message) return '';

    // 1. Переводим строку в байты UTF-8
    const msgBuffer = new TextEncoder().encode(message);

    // 2. Вычисляем хэш (возвращает ArrayBuffer)
    const hashBuffer = await crypto.subtle.digest('SHA-256', msgBuffer);

    // 3. Безопасная конвертация ArrayBuffer в Base64 строку
    const bytes = new Uint8Array(hashBuffer);
    let binString = '';
    for (let i = 0; i < bytes.byteLength; i++) {
        binString += String.fromCharCode(bytes[i]);
    }

    return btoa(binString);
}