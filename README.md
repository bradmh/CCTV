# Monitoring
Aplikacja dla operatorów monitoringu

Aplikacja do działania wymaga stworzonej wcześniej bazy MySQL (CREATE DATABASE monitoring).
Po naciśnięciu F7 na ekranie logowania w bazie zostaną stworzone potrzebne kolumny i wprowadzone zostaną podstawowe dane.

W pliku db.cs jest statyczna klasa DbCreds zawierająca dane połączenia.
W kodzie nie ma portu, działa na domyślnym 3306.

Testowane na MariaDB 10.3 64bit.
