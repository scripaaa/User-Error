import requests

cookies = {
    "MoodleSession": "rhphob4ahlp9v4utr37kdgmfa6",   # ← вставьте своё значение!
    "MOODLEID_": "l%251D%25C6q%25D9",       # ← и это
}

headers = {
    "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.6723.92 Safari/537.36",
}

def check_with_auth(page_id: int) -> bool:
    url = f"https://edu.mmcs.sfedu.ru/mod/quiz/view.php?id={page_id}"
    try:
        resp = requests.get(url, cookies=cookies, headers=headers, timeout=10)
        # Доп. проверка: нет ли редиректа на login
        if "login" in resp.url or resp.status_code in (302, 403):
            return False
        return resp.status_code == 200
    except Exception as e:
        print(f"Ошибка: {e}")
        return False

print(check_with_auth(33405))  # Должно быть True