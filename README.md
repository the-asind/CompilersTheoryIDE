# IDE. Компилятор 
- [I. Разработка пользовательского интерфейса (GUI) для языкового процессора.](#часть-1-разработка-пользовательского-интерфейса-gui-для-языкового-процессора)
- [II. Разработка лексического анализатора (сканера).](#часть-2-разработка-лексического-анализатора-сканера)
- [III. Разработка синтаксического анализатора (парсера).](#часть-3-разработка-синтаксического-анализатора-парсера)
- [IV. Нейтрализация ошибок (метод Айронса).](#часть-4-нейтрализация-ошибок-метод-айронса)

----

## Часть 1. Разработка пользовательского интерфейса (GUI) для языкового процессора.
**Цель:** Разработать приложение – текстовый редактор.

![Скриншот работы программы. Подсветка идентична ЯП Python.](https://github.com/the-asind/CompilersTheoryIDE/assets/84527186/08ac1365-9496-4377-9872-eab591455bfa)

_Рис. 1. Скриншот работы программы. Подсветка идентична ЯП Python._

![image](https://github.com/the-asind/CompilersTheoryIDE/assets/84527186/cddcf1c9-2835-48ab-9cfc-9b273c38b625)

_Рис. 2. Открыт на полный экран, изменение размера текста через меню._

![компилятор 1](https://github.com/the-asind/CompilersTheoryIDE/assets/84527186/99867d7f-4536-4c51-99ea-e500bf6f3c07)

_Рис. 3. Анимация. Работа меню инструментов. Изменение текста._

![image](https://github.com/the-asind/CompilersTheoryIDE/assets/84527186/4483a8ff-2a88-41b8-83ce-35f93963851f)

_Рис. 4. Демонстрация таблицы ошибок с мокап-данными для примера._

![компилятор 2](https://github.com/the-asind/CompilersTheoryIDE/assets/84527186/f855da0d-de51-4ad4-bb49-04c23175b208)

_Рис. 5. Анимация. Демонстративная работа с файлами._

![image](https://github.com/the-asind/CompilersTheoryIDE/assets/84527186/5b69c160-fb39-4897-8a64-07e1984a788d)

_Рис. 6. Справка. Помощь._

![image](https://github.com/the-asind/CompilersTheoryIDE/assets/84527186/d154be84-71bd-4f07-b723-7ea2f03d5eb7)

_Рис. 7. Справка. О программе_

![image](https://github.com/the-asind/CompilersTheoryIDE/assets/84527186/a27ed87f-fdd7-4d84-958a-7c60ceeb7f4e) ![image](https://github.com/the-asind/CompilersTheoryIDE/assets/84527186/c125aa9e-6017-4eac-86a7-43b5d128d237)

_Рис. 8 (а, б). Локализация. Английский и русский языки._

----

## Часть 2. Разработка лексического анализатора (сканера).

### Цель: 
  Изучить назначение лексического анализатора. Спроектировать алгоритм и выполнить программную реализацию сканера.

### Постановка задачи: 
1. Спроектировать диаграмму состояний сканера (примеры диаграмм представлены в прикрепленных файлах).
2. Разработать лексический анализатор, позволяющий выделить в тексте лексемы, иные символы считать недопустимыми (выводить ошибку).
3. Встроить сканер в ранее разработанный интерфейс текстового редактора. Учесть, что текст для разбора может состоять из множества строк.

### Примеры допустимых строк:

```python
# example
```

```python
"""
example
"""
```

```python
'''
example
'''
```

```python
# example
""" example '' """
''' "
example" '''
```

### Тема 51
- Комментарии языка Python.

### Диаграмма состояния сканера:

![диаграмма состояния сканера](https://github.com/the-asind/CompilersTheoryIDE/assets/84527186/389be05d-bcd2-47e6-a876-aadb663d0ba6)

_Рис. 9. Диаграмма состояния сканера_

### Тестовые данные:

![image](https://github.com/the-asind/CompilersTheoryIDE/assets/84527186/36a08a21-1f9c-4157-a07c-205c1f551d65) ![image](https://github.com/the-asind/CompilersTheoryIDE/assets/84527186/85242cad-00d1-43dc-b652-615970c82c39)

_Рис. 10(а,б). Сложный пример работы сканнера. а - английская локализация, б - русская локализация._

![image](https://github.com/the-asind/CompilersTheoryIDE/assets/84527186/9e666a70-2345-4224-9bc7-22c338c59ec5)

_Рис. 11. Простой пример работы выделения типа лексемы._

----

## Часть 3. Разработка синтаксического анализатора (парсера).
**Цель:** Изучить назначение синтаксического анализатора. Спроектировать алгоритм и выполнить программную реализацию парсера.

> [!IMPORTANT]
> Тема 51. Комментарии языка Python.

### Грамматика

**G** [&lt;I&gt;]:

**V<sub>T</sub>** = { ‘#’, ‘"’, ‘'’, newline, symbol}

**V<sub>N</sub>** = { &lt;I&gt;, FIRSTOPENSINGLEQUOTE, FIRSTOPENSINGLEQUOTE, SECONDOPENSINGLEQUOTE, THIRDOPENSINGLEQUOTE, MULTILINESINGLEQUOTESCOMMENT, FIRSTSINGLECLOSEQUOTE, SECONDSINGLECLOSEQUOTE, FIRSTOPENDOUBLEQUOTE, SECONDOPENDOUBLEQUOTE, THIRDOPENDOUBLEQUOTE, MULTILINEDOUBLEQUOTESCOMMENT, FIRSTDOUBLECLOSEQUOTE, SECONDDOUBLECLOSEQUOTE, SHARP, SINGLELINECOMMENT}

**P** = 
{
- I → ' FIRSTOPENSINGLEQUOTE | " FIRSTOPENDOUBLEQUOTE | # SHARP
- FIRSTOPENSINGLEQUOTE → ' SECONDOPENSINGLEQUOTE
- SECONDOPENSINGLEQUOTE → ' THIRDOPENSINGLEQUOTE
- THIRDOPENSINGLEQUOTE → symbol MULTILINESINGLEQUOTESCOMMENT
- MULTILINESINGLEQUOTESCOMMENT → symbol MULTILINESINGLEQUOTESCOMMENT | ' FIRSTSINGLECLOSEQUOTE
- FIRSTSINGLECLOSEQUOTE → ' SECONDSINGLECLOSEQUOTE
- SECONDSINGLECLOSEQUOTE → '
- FIRSTOPENDOUBLEQUOTE  → " SECONDOPENDOUBLEQUOTE 
- SECONDOPENDOUBLEQUOTE → " THIRDOPENDOUBLEQUOTE 
- THIRDOPENDOUBLEQUOTE → symbol MULTILINEDOUBLEQUOTESCOMMENT 
- MULTILINEDOUBLEQUOTESCOMMENT -> symbol MULTILINEDOUBLEQUOTESCOMMENT | " FIRSTDOUBLECLOSEQUOTE
- FIRSTDOUBLECLOSEQUOTE → " SECONDDOUBLECLOSEQUOTE
- SECONDDOUBLECLOSEQUOTE → "
- SHARP → symbol SINGLELINECOMMENT
- SINGLELINECOMMENT → symbol SINGLELINECOMMENT | newline

}

Грамматика является полностью автоматной, (по классификации Хомского)

### Граф конечного автомата

![граф конечного автомата](https://github.com/the-asind/CompilersTheoryIDE/assets/84527186/3674f4b6-08ee-4d48-9596-7073d501bf61)

_Рис. 12. Граф конечного автомата комментариев на Python._

----

## Часть 4. Нейтрализация ошибок (метод Айронса).
**Цель:** Реализовать алгоритм нейтрализации синтаксических ошибок и дополнить им программную реализацию парсера.

> [!IMPORTANT]
> Тема 51. Комментарии языка Python.
:bowtie:

----
