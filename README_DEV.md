Інструкція для розробників

Цей репозиторій містить дві окремі гілки для двох розробників: Rostik і Yarik.  
Основна гілка — main (в неї напряму пушити *заборонено*).  
___________________________________________________________________________________


 -- Для Розробника №1 (Rostik)

1. Клонувати репозиторій:
   - git clone https://github.com/itstepedu/answer_ua.git
      
2. Переключитися на свою гілку:
   - git checkout Rostik

3. Додавати та комітити зміни:
   - git add .
   - git commit -m "Мій перший коміт у Rostik"
   - git push origin Rostik

4. Надсилати зміни в main тільки через *Pull Request*:

   - Перейти на GitHub → Pull requests → *New pull request*  
   - Вибрати *base: main* ← *compare: Rostik*  
   - Створити PR.  
   - Адміністратор після рев’ю об’єднає код.
______________________________________________________________________


-- Для Розробника №2 (Yarik)

1. Клонувати репозиторій:
   - git clone https://github.com/itstepedu/answer_ua.git
      
2. Переключитися на свою гілку:
   - git checkout Yarik

3. Додавати та комітити зміни:
   - git add .
   - git commit -m "Мій перший коміт у Yarik"
   - git push origin Yarik

4. Надсилати зміни в main тільки через *Pull Request*:

   - Перейти на GitHub → Pull requests → *New pull request*  
   - Вибрати *base: main* ← *compare: Yarik*  
   - Створити PR.  
   - Адміністратор після рев’ю об’єднає код.

