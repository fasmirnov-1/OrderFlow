const CRYTPO_KEY_STR = "MySuperSecretKeyForEncryption123";
const SERVER_IP = "http://194.87.x.x:5000"; // Шлем напрямую на IP (без домена)

async function getCryptoKey() {
    const enc = new TextEncoder();
    return await window.crypto.subtle.importKey(
        "raw", enc.encode(CRYTPO_KEY_STR),
        { name: "AES-GCM" }, false, ["encrypt"]
    );
}

async function encryptString(text, key) {
    const iv = window.crypto.getRandomValues(new Uint8Array(12));
    const enc = new TextEncoder();
    const encryptedBuffer = await window.crypto.subtle.encrypt(
        { name: "AES-GCM", iv: iv }, key, enc.encode(text)
    );

    const encryptedBytes = new Uint8Array(encryptedBuffer);
    const base64Data = btoa(String.fromCharCode.apply(null, encryptedBytes));
    const base64Iv = btoa(String.fromCharCode.apply(null, iv));

    return `${base64Iv}:${base64Data}`; // Формат IV:CipherText
}

// Универсальная функция отправки пакета с зашифрованным URL
async function sendTunnelRequest(realPathWithQuery, bodyObject) {
    const key = await getCryptoKey();

    // 1. Шифруем НАПРАВЛЕНИЕ (URL-путь)
    const encryptedPath = await encryptString(realPathWithQuery, key);

    // 2. Шифруем ТЕЛО запроса
    const plainTextJson = JSON.stringify(bodyObject);
    const encryptedBody = await encryptString(plainTextJson, key);

    // Отправляем пакет. В сети пишется только IP-адрес и корень "/"
    const response = await fetch(`${SERVER_IP}/`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "X-Encrypted-Path": encryptedPath // Передаем зашифрованный URL в заголовке
        },
        body: JSON.stringify({ payload: encryptedBody })
    });

    return response;
}