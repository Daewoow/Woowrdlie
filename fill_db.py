
connection_string = "Host=localhost;Port=5432;Database=WordlieDB;Username=postgres;Password=ifconfigroute-n;"
import psycopg2
from psycopg2 import sql

DB_NAME = "WordlieDB"
DB_USER = "postgres"
DB_PASSWORD = "ifconfigroute-n"
DB_HOST = "localhost"
DB_PORT = "5432"

WORDS_FILE_PATH = "singular.txt"

def insert_words_from_file():
    try:
        conn = psycopg2.connect(
            dbname=DB_NAME,
            user=DB_USER,
            password=DB_PASSWORD,
            host=DB_HOST,
            port=DB_PORT
        )
        cursor = conn.cursor()

        with open(WORDS_FILE_PATH, encoding='utf-8') as file:
            words = [line.strip() for line in file if line.strip()]

        insert_query = sql.SQL("INSERT INTO public.\"Words\" (\"Word\") VALUES {}").format(
            sql.SQL(',').join([sql.SQL("(%s)") for _ in words])
        )

        cursor.execute(insert_query, words)
        conn.commit()

        print(f"Успешно добавлено {len(words)} слов в таблицу Words.")

    except Exception as e:
        print(f"Ошибка: {e}")
        if conn:
            conn.rollback()
    finally:
        if cursor:
            cursor.close()
        if conn:
            conn.close()

insert_words_from_file()