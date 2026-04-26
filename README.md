[README.md](https://github.com/user-attachments/files/27105745/README.md)
# 🚌 BusNow Plovdiv

**Уеб приложение за информация за градския обществен транспорт в Пловдив**

[![License: AGPL v3](https://img.shields.io/badge/License-AGPL%20v3-blue.svg)](https://www.gnu.org/licenses/agpl-3.0)
[![.NET](https://img.shields.io/badge/.NET-ASP.NET%20Core-purple)](https://dotnet.microsoft.com/)
[![Language: C#](https://img.shields.io/badge/Language-C%23-green)](https://docs.microsoft.com/en-us/dotnet/csharp/)

---

## 📋 Описание

**BusNow Plovdiv** е уеб приложение, което предоставя актуална информация за маршрутите и разписанията на градския обществен транспорт в Пловдив. Проектът е разработен като дипломна работа и е изграден върху принципите на **Clean Architecture**, което осигурява ясно разделение на отговорностите и лесна поддръжка.

---

## 🗂️ Структура на проекта

Проектът е организиран по модела на **Clean Architecture** и се състои от следните слоеве:

```
BusNowPLovdiv/
├── BusNow.Core/            # Домейн слой – бизнес логика, ентитети, интерфейси
├── BusNow.Application/     # Приложен слой – use cases, DTO, услуги
├── BusNow.Infrastructure/  # Инфраструктурен слой – база данни, репозитории
├── BusNow.Web/             # Презентационен слой – ASP.NET Core MVC, контролери, изгледи
├── BusNow.Tests/           # Unit и интеграционни тестове
└── BusNow.sln              # Solution файл
```

### Описание на слоевете

| Слой | Описание |
|------|----------|
| **Core** | Съдържа домейн ентитетите и интерфейсите, без никакви зависимости от външни библиотеки |
| **Application** | Имплементира бизнес логиката чрез услуги, използвайки интерфейсите от Core |
| **Infrastructure** | Отговаря за достъпа до данни (база данни, външни API-та) |
| **Web** | Уеб интерфейсът – контролери, Razor изгледи, CSS/JS |
| **Tests** | Тестове за валидиране на функционалността |

---

## 🛠️ Технологии

- **Платформа:** ASP.NET Core (C#)
- **Архитектура:** Clean Architecture
- **Изгледи:** Razor Pages / MVC (HTML, CSS, JavaScript)
- **ORM:** Entity Framework Core
- **Тестове:** xUnit
- **Лиценз:** GNU AGPL v3

---

## ⚙️ Изисквания

За да стартирате проекта локално, са необходими:

- [.NET SDK 8.0+](https://dotnet.microsoft.com/download)
- SQL Server
- IDE: [Visual Studio 2022](https://visualstudio.microsoft.com/)

---

## 🚀 Стартиране на проекта

1. **Клонирайте репозитория:**
   ```bash
   git clone https://github.com/SavaKufarov/BusNowPLovdiv.git
   cd BusNowPLovdiv
   ```

2. **Конфигурирайте connection string** в `BusNow.Web/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=...;Database=BusNowDb;..."
     }
   }
   ```

3. **Приложете миграциите:**
   ```bash
   dotnet ef database update --project BusNow.Infrastructure --startup-project BusNow.Web
   ```

4. **Стартирайте приложението:**
   ```bash
   cd BusNow.Web
   dotnet run
   ```

5. Отворете браузър на `https://localhost:5001`

---

## 🧪 Стартиране на тестовете

```bash
dotnet test BusNow.Tests
```

---

## 📌 Функционалности

- 📍 Преглед на автобусни линии в Пловдив
- 🕐 Разписания по спирки и маршрути
- 🔍 Търсене на конкретна линия или спирка
- 📱 Responsive дизайн – работи на телефон и компютър

---

## 👤 Автор

**Сава Куфаров**
- GitHub: [@SavaKufarov](https://github.com/SavaKufarov)

---

## 📄 Лиценз

Този проект е лицензиран под **GNU Affero General Public License v3.0**.
Вижте файла [LICENSE.txt](LICENSE.txt) за повече информация.
