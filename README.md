# StudyBuddy

## Avtorji
Ajla Suljanović 63220443

Enrik Roža 63240381

## Kratek opis aplikacije

StudyBuddy je spletna aplikacija ASP.NET MVC, ki študentom pomaga lažje organizirati učenje, najti študijske materiale, tutorje in študijske skupine ter komunicirati prek foruma.

StudyBuddy je platforma za študente, ki omogoča:

- pregled in dodajanje študijskih gradiv (povezave, zapiski, PDF-i, vaje),
- pregled tutorjev in sistem »help points« za nagrajevanje pomoči,
- organizacijo study sessionov (termini za skupno učenje),
- forum za vprašanja, odgovore in izmenjavo nasvetov,
- prijavo in registracijo uporabnikov (avtentikacija z Identity).

Aplikacija je zasnovana kot centralno mesto, kjer študenti lahko najdejo gradiva, pomoč in skupnost za lažje in bolj strukturirano učenje.

## Tehnologije

- **Backend:** ASP.NET Core MVC (.NET 9), Entity Framework Core, Identity
- **Frontend:** Razor Views, Bootstrap 5, custom CSS (modern, pastel UI)
- **Baza:** PostgreSQL (Docker, Npgsql provider)
- **Ostalo:** EF Core migracije, seeding začetnih podatkov

## Funkcionalnosti (trenutno implementirane)

- Landing stran / Home z modernim UI (hero sekcija, "Why StudyBuddy?", "How it works?")
- Prijava in registracija uporabnikov (Identity)
- Prikaz imena prijavljenega uporabnika v navigaciji
- Pregled tutorjev + filtriranje po predmetu
- Sistem "help points" za tutorje
- Top 3 tutorji prikazani na Home strani
- Seznam študijskih gradiv (Materials) + filtriranje po predmetu in iskanju
- Dodajanje novega gradiva (link) v bazo
- Seznam study sessionov (Study Sessions) + filtriranje po datumu in predmetu
- Dodajanje novega study sessiona
- Forum (seznam threadov) z informacijami o kategoriji, predmetu, avtorju in številom odgovorov
- Podatkovni model z več povezanimi entitetami (Faculty, Subject, Tutor, TutorSubject, Material, StudyPost, ForumThread)

## Podatkovni model (entitete)

Trenutno uporabljamo naslednje entitete:

- `Faculty` – fakultete (npr. FRI)
- `Subject` – predmeti (IS, APS1, …)
- `Tutor` – tutorji (ime, fakulteta, help points)
- `TutorSubject` – povezava med tutorjem in predmeti
- `Material` – študijsko gradivo (naslov, opis, tip, url, predmet, avtor)
- `StudyPost` – study session (naslov, predmet, lokacija/online, datum in čas, avtor)
- `ForumThread` – forum teme (naslov, vsebina, kategorija, predmet, avtor, število odgovorov)

Vsi podatki se shranjujejo v **PostgreSQL** bazo preko Entity Framework Core.

## Nastavitev in zagon projekta

### 1. Zagon PostgreSQL baze (Docker)

```bash
docker run --name studybuddy-postgres \
  -e POSTGRES_PASSWORD=pass123 \
  -e POSTGRES_DB=studybuddy \
  -p 5432:5432 \
  -d postgres:16
