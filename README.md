# RACEDAY-OVERVIEW-POE
Raceday POE Part 1 - System Planning and Database 

Project Overview

RaceDay is a full‑stack event management platform built for South Africa’s running, walking, and cycling community. The idea is simple: **Event Organisers** can set up and manage events, categories, and results, while **Participants** can browse upcoming races, sign up, track their results, and even check live weather updates to prepare for race day.

This repo contains the planning work for Part 1 — all the groundwork before any code was written. It covers the database design, the API endpoint plan, and the SQL script used to create and seed the database.

Repository Structure

docs/
/ raceday_erd.png          # Entity Relationship Diagram (Section A)
/ API_Endpoint_Plan.md     # Full API endpoint plan (Section B)
/ RaceDay_Schema.sql       # SQL Server schema + seed data (Section C)

Section A — Entity Relationship Diagram

The ERD (`docs/raceday_erd.png`) shows the full data model for RaceDay. There are **7 entities** in total: Roles, Users, Events, Categories, Enrolments, Results, and WeatherSnapshots. Each table has its primary keys, foreign keys, and relationship cardinalities clearly labelled.

Section B — API Endpoint Plan

The API plan (`docs/API_Endpoint_Plan.md`) lists all endpoints across Authentication, User Profiles, Events, Categories, Enrolments, and Results. For each endpoint, I included the HTTP method, route, description, required role, request body, and expected response.

Section C — SQL Database Script

The SQL script (`docs/RaceDay_Schema.sql`) was written and tested in SQL Server Management Studio (SSMS). Running it is straightforward:

1. Open SSMS and connect to your local SQL Server instance  
2. Open `RaceDay_Schema.sql`  
3. Execute the script (F5) — this creates the `RaceDayDB` database, all 7 tables with constraints, and seeds it with sample data (2 Organisers, 2 Participants, 3 Events, 5 Categories, sample Enrolments, a Result, and a Weather snapshot)

ERD/SQL alignment
The SQLtches the ERD exactly — no differences.
