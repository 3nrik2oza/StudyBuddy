# StudyBuddy

## Avtorji
Ajla Suljanović 63220443

Enrik Roža 63240381

## Kratek opis projekta

StudyBuddy je spletna aplikacija, razvita v ogrodju **ASP.NET Core MVC**, namenjena študentom za lažjo organizacijo učenja, iskanje študijskih gradiv, tutorjev, študijskih skupin ter komunikacijo prek foruma.

Aplikacija deluje kot centralna platforma, kjer lahko študenti:
- dostopajo do študijskih gradiv,
- najdejo tutorje in zaprosijo za pomoč,
- organizirajo ali se pridružijo study sessionom,
- sodelujejo v forumu,
- upravljajo svojo uporabniško identiteto (prijava/registracija).

---

## Funkcionalnosti sistema

### Spletna aplikacija
- prijava in registracija uporabnikov (ASP.NET Identity),
- pregled in dodajanje študijskih gradiv (Materials),
- pregled tutorjev + sistem »help points«,
- pošiljanje tutoring zahtevkov in komunikacija,
- organizacija study sessionov (StudyPosts),
- forum s temami in odgovori,
- filtriranje vsebin po predmetih,
- moderna in dodelana uporabniška izkušnja (custom CSS + Bootstrap).

### Spletna storitev (REST API)
- REST API razvit v .NET,
- JSON komunikacija,
- CRUD operacije nad entitetami,
- avtentikacija in avtorizacija,
- Swagger UI dokumentacija.

---

## Tehnologije

- **Backend:** ASP.NET Core MVC (.NET 9)
- **ORM:** Entity Framework Core
- **Avtentikacija:** ASP.NET Identity
- **Baza podatkov:** PostgreSQL (Azure)
- **Frontend:** Razor Views, Bootstrap 5, custom CSS
- **API dokumentacija:** Swagger / OpenAPI

---

## Podatkovna baza

Podatkovna baza vsebuje več kot 5 tabel, med drugim:

- AspNetUsers (Identity)
- Faculties
- Subjects
- Materials
- Tutors
- TutorSubjects
- TutorRequests
- TutorRequestMessages
- StudyPosts
- StudyPostParticipants
- ForumThreads
- ForumReplies
- Bookmarks

Ista PostgreSQL baza se uporablja:
- v spletni aplikaciji,
- v REST spletni storitvi.

---

## Javni dostop

- **Spletna aplikacija:**  
  👉 https://studdybuddyapp.azurewebsites.net/

- **REST API (Swagger):**  
  👉 https://studdybuddyapp.azurewebsites.net/swagger/index.html

---

## GitHub repozitoriji

- **Web aplikacija + API:**  
  👉 https://github.com/3nrik2oza/StudyBuddy/edit/main

- **Android aplikacija:**  
  👉 https://github.com/3nrik2oza/StudyBuddyMobile

---

## Zaslonske slike (screenshots)

Screenshoti uporabniškega vmesnika so dodani v mapo:
web/wwwroot/images

