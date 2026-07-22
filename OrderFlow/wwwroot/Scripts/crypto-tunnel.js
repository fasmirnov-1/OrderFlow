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
// Абсолютно безопасная функция настройки без падений на HTTP
function runBrowserCryptoFix() {
    const origin = window.location.origin;
    const flagsUrl = "chrome://flags/#unsafely-treat-insecure-origin-as-secure";
    const userAgent = navigator.userAgent;

    const isFirefox = userAgent.indexOf("Firefox") > -1;
    const isEdge = userAgent.indexOf("Edg") > -1;
    const isOpera = userAgent.indexOf("OPR") > -1 || userAgent.indexOf("Opera") > -1;
    const isChrome = userAgent.indexOf("Chrome") > -1 && !isEdge && !isOpera;

    // Безопасная попытка записи в буфер с защитой от отсутствия API на HTTP
    try {
        if (navigator.clipboard && typeof navigator.clipboard.writeText === 'function') {
            navigator.clipboard.writeText(origin).catch(() => { });
        }
    } catch (err) {
        // Игнорируем ограничения браузера
    }

    if (isFirefox) {
        alert(
            "Инструкция для Firefox:\n\n" +
            "1. Введите в адресной строке: about:config и нажмите Enter.\n" +
            "2. Найдите параметр security.mixed_content.block_active_content.\n" +
            "3. Для разработки используйте localhost или настройте HTTPS."
        );
    } else if (isChrome || isEdge || isOpera) {
        alert(
            "Адрес сайта: " + origin + "\n\n" +
            "Инструкция по настройке Chromium (Chrome, Edge, Opera):\n" +
            "1. Скопируйте адрес сайта вручную.\n" +
            "2. Откройте в новой вкладке адрес:\n" + flagsUrl + "\n" +
            "3. Найдите пункт 'Insecure origins treated as secure'.\n" +
            "4. Вставьте адрес сайта, переведите в положение 'Enabled' и нажмите Relaunch."
        );
        window.prompt("Скопируйте ссылку на флаги браузера:", flagsUrl);
    } else {
        alert(
            "Адрес сайта: " + origin + "\n\n" +
            "Ваш браузер блокирует Web Crypto API по протоколу HTTP. Рекомендуется использовать HTTPS."
        );
    }
}
// Функция настройки для мобильных браузеров с определением платформы
function runMobileCryptoFix() {
    const origin = window.location.origin;
    const flagsUrl = "chrome://flags/#unsafely-treat-insecure-origin-as-secure";
    const userAgent = navigator.userAgent || navigator.vendor || window.opera;
    const isAndroid = /android/i.test(userAgent);

    const container = document.getElementById('mobileCryptoFixContainer');
    const input = document.getElementById('mobileFlagsUrlInput');

    if (container && input) {
        // Делаем блок видимым, чтобы элемент стал частью DOM и текста
        input.value = flagsUrl;
        container.style.display = "block";

        // Надежный метод выделения и копирования для мобильных устройств
        input.focus();
        input.select();
        input.setSelectionRange(0, 99999); // Для мобильных iOS/Android

        let success = false;
        try {
            success = document.execCommand('copy');
        } catch (err) {
            success = false;
        }

        // Дублируем попытку через современный API на всякий случай
        try {
            if (navigator.clipboard && navigator.clipboard.writeText) {
                navigator.clipboard.writeText(flagsUrl);
                success = true;
            }
        } catch (err) { }

        if (success) {
            alert("Ссылка на настройки флагов успешно скопирована в буфер обмена!\n\nОткройте новую вкладку в браузере, вставьте ссылку в адресную строку и добавьте сайт в доверенные.");
        } else {
            alert("Поле со ссылкой отображено под кнопкой. Выделите и скопируйте её вручную.");
        }
    }
}