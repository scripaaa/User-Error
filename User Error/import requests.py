import requests
from bs4 import BeautifulSoup
import time
import re

# 🔐 1. Ваши cookies (скопированы из Chrome DevTools → Application → Cookies)
COOKIES = {
    "MoodleSession": "rhphob4ahlp9v4utr37kdgmfa6",   # ← ваша сессия
    "MOODLEID_": "l%251D%25C6q%25D9",                # ← ваш ID
}

# 🖥️ 2. Заголовки как у Chrome — чтобы не выдавало, что это бот
HEADERS = {
    "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.6723.92 Safari/537.36",
    "Accept": "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8",
    "Accept-Language": "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7",
    "Accept-Encoding": "gzip, deflate, br",
    "Connection": "keep-alive",
    "Upgrade-Insecure-Requests": "1",
    "Sec-Fetch-Dest": "document",
    "Sec-Fetch-Mode": "navigate",
    "Sec-Fetch-Site": "same-origin",
    "Sec-Fetch-User": "?1",
}

# 🔍 3. Что ищем на странице? (меняйте под ваш курс!)
# Можно использовать текст, часть заголовка, или регулярное выражение
TARGET_PATTERNS = [
    "Занятие #20. Последовательности",        # название курса
    "Занятие #20.Последовательности",                         # тип активности
    "Занятие #20",                      # например, "конечный автомат"
    # "осень 2024",                 # год/семестр — раскомментируйте, если нужно уточнить
]

# Или строгая проверка по title (регистронезависимо)
TITLE_REGEX = re.compile(r"Дискретная\s+математика|Теория\s+автоматов", re.IGNORECASE)

# 🌐 4. Базовый URL (quiz — тесты, assign — задания)
BASE_URL = "https://edu.mmcs.sfedu.ru/mod/quiz/view.php?id="  # ← меняйте на .../assign/... если нужно

# 🔢 5. Диапазон ID для перебора
START_ID = 11000
END_ID = 12000


def is_target_page(page_id: int) -> bool:
    """
    Проверяет, что страница:
    - доступна (200, без редиректа на login),
    - содержит ключевые слова из TARGET_PATTERNS или TITLE_REGEX.
    """
    url = BASE_URL + str(page_id)
    try:
        resp = requests.get(
            url,
            cookies=COOKIES,
            headers=HEADERS,
            timeout=10
        )

        # 🚫 Если редирект на login или ошибка доступа — не наша страница
        if resp.status_code in (302, 403, 401) or "login" in resp.url:
            return False

        if resp.status_code != 200:
            return False

        soup = BeautifulSoup(resp.text, "html.parser")

        # 🔎 1. Проверка <title>
        title_tag = soup.find("title")
        title_text = title_tag.get_text().strip() if title_tag else ""
        if TITLE_REGEX.search(title_text):
            return True

        # 🔎 2. Проверка заголовков h1/h2/h3
        for header in soup.find_all(['h1', 'h2', 'h3']):
            if any(pattern in header.get_text() for pattern in TARGET_PATTERNS):
                return True

        # 🔎 3. Проверка любого текста на странице (осторожно: может быть много ложных совпадений)
        page_text = soup.get_text()
        if any(pattern in page_text for pattern in TARGET_PATTERNS):
            return True

        return False

    except Exception as e:
        print(f"[!] Ошибка при проверке {page_id}: {e}")
        return False


def scan_quizzes():
    found = []
    print(f"🔍 Начинаю поиск в диапазоне ID [{START_ID}–{END_ID}]...")
    print("-" * 60)

    for pid in range(START_ID, END_ID + 1):
        print(f"Проверяю id={pid:5d}...", end=" ", flush=True)

        if is_target_page(pid):
            print("✅ НАЙДЕНО!")
            found.append(pid)
        else:
            print("—")

        # ⏱️ Вежливая пауза (можно уменьшить до 0.3, если сайт не блокирует)
        time.sleep(0.3)

    return found


if __name__ == "__main__":
    results = scan_quizzes()

    print("\n" + "="*60)
    if results:
        print(f"🎯 Найдено {len(results)} тест(ов)/заданий по ключевым словам:")
        for pid in results:
            print(f"  → {BASE_URL}{pid}")
    else:
        print("❌ Ничего не найдено. Проверьте:")
        print("   - Актуальны ли cookies?")
        print("   - Правильный ли диапазон ID?")
        print("   - Корректны ли TARGET_PATTERNS / TITLE_REGEX?")