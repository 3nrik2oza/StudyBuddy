# StudyBuddy

## Avtorji

* Ajla Suljanović – 63220443
* Enrik Roža – 63240381

---

# Kratek opis projekta

StudyBuddy je spletna aplikacija, razvita v ogrodju ASP.NET Core MVC, namenjena študentom za lažjo organizacijo učenja, iskanje študijskih gradiv, tutorjev, študijskih skupin ter komunikacijo prek foruma.

Aplikacija deluje kot centralna platforma, kjer lahko študenti:

* dostopajo do študijskih gradiv,
* najdejo tutorje in zaprosijo za pomoč,
* organizirajo ali se pridružijo study sessionom,
* sodelujejo v forumu,
* upravljajo svojo uporabniško identiteto (prijava/registracija).

---

# Funkcionalnosti sistema

## Spletna aplikacija

* prijava in registracija uporabnikov (ASP.NET Identity),
* pregled in dodajanje študijskih gradiv (Materials),
* pregled tutorjev + sistem »help points«,
* pošiljanje tutoring zahtevkov in komunikacija,
* organizacija study sessionov (StudyPosts),
* forum s temami in odgovori,
* filtriranje vsebin po predmetih,
* moderna uporabniška izkušnja (Bootstrap + custom CSS),
* REST API integracija,
* Swagger dokumentacija.

## REST API

* CRUD operacije nad entitetami,
* JSON komunikacija,
* avtentikacija in avtorizacija,
* Swagger/OpenAPI dokumentacija.

---

# Tehnologije

* Backend: ASP.NET Core MVC (.NET 9)
* ORM: Entity Framework Core
* Avtentikacija: ASP.NET Identity
* Podatkovna baza: PostgreSQL
* Frontend: Razor Views, Bootstrap 5, custom CSS
* API dokumentacija: Swagger / OpenAPI
* Deployment: Azure App Service
* Docker: PostgreSQL container

---

# Podatkovna baza

Podatkovna baza vsebuje več tabel, med drugim:

* AspNetUsers
* Faculties
* Subjects
* Materials
* Tutors
* TutorSubjects
* TutorRequests
* TutorRequestMessages
* StudyPosts
* StudyPostParticipants
* ForumThreads
* ForumReplies
* Bookmarks

Ista PostgreSQL baza se uporablja:

* v spletni aplikaciji,
* v REST API storitvi.

---

# Lokalni zagon aplikacije

## Zahteve

* .NET 9 SDK
* Docker Desktop
* PostgreSQL container

## Zagon PostgreSQL containerja

```bash
docker start studybuddy-postgres
```

## Zagon aplikacije

```bash
cd web
dotnet restore
dotnet run
```

## Lokalni dostop

```txt
http://localhost:5027
```

---

# Javni dostop

## Spletna aplikacija

https://studdybuddyapp.azurewebsites.net/

## Swagger API

https://studdybuddyapp.azurewebsites.net/swagger/index.html

---

# GitHub repozitoriji

## Web aplikacija + API

https://github.com/3nrik2oza/StudyBuddy

## Android aplikacija

https://github.com/3nrik2oza/StudyBuddyMobile

---

# Screenshoti

Screenshoti uporabniškega vmesnika so dodani v mapo:

```txt
web/wwwroot/images
```
