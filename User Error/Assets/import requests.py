import requests
from bs4 import BeautifulSoup
import time
import re





BASE_URL = "https://edu.mmcs.sfedu.ru/mod/assign/view.php?id="

# Настройки
START_ID = 33404
END_ID = 33410

# Что искать — можно использовать текст, часть title, или регулярное выражение
TARGET_PATTERNS = [
    "Занятие #20. Последовательности",
    "Контрольная работа #3"   
     # часть названия курса
     # тема задания
    # Можно добавить больше условий
]

# Или строго по title (через регулярку)
TITLE_REGEX = re.compile(r"Математическая логика", re.IGNORECASE)

HEADERS = {
    "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"
}

def page_belongs_to_course(page_id: int) -> bool:
    url = BASE_URL + str(page_id)
    try:
        response = requests.get(url, headers=HEADERS, timeout=10)
        
        # Если статус не 200 — сразу False
        if response.status_code != 200:
            return False

        soup = BeautifulSoup(response.text, 'html.parser')

        # Вариант 1: проверить <title>
        title_tag = soup.find('title')
        if title_tag and TITLE_REGEX.search(title_tag.get_text()):
            return True

        # Вариант 2: проверить заголовки h1/h2
        for header in soup.find_all(['h1', 'h2', 'h3']):
            text = header.get_text().strip()
            if any(pattern in text for pattern in TARGET_PATTERNS):
                return True

        # Вариант 3: проверить URL курса в ссылках (часто в хлебных крошках)
        # Например: <a href=".../course/view.php?id=789">МЛиТА</a>
        for link in soup.find_all('a', href=True):
            if 'course/view.php?id=' in link['href']:
                link_text = link.get_text()
                if any(p in link_text for p in TARGET_PATTERNS):
                    return True

        return False

    except Exception as e:
        print(f"[!] Ошибка при обработке {url}: {e}")
        return False

def scan_for_course():
    matches = []
    for pid in range(START_ID, END_ID + 1):
        print(f"Проверяю id={pid}...", end=" ")
        is_target = page_belongs_to_course(pid)
        status = "✅ Нашёл!" if is_target else "—"
        print(status)
        if is_target:
            matches.append(pid)
          # вежливая пауза
    return matches

if __name__ == "__main__":
    print("🔍 Поиск заданий по курсу...")
    found = scan_for_course()

    print("\n🎯 Найдены ID заданий по нужному курсу:")
    for pid in found:
        print(f"  → {BASE_URL}{pid}")