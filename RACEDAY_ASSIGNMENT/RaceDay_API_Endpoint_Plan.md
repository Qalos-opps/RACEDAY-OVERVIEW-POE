# RaceDay API Endpoint Plan

**Module:** PROG6212 POE - Part 1  
**System:** RaceDay  
**Purpose:** Planned RESTful API endpoints for authentication, user profiles, events, categories, enrolments, results, and weather.

| HTTP Method | Route | Description | Role Required | Request Body | Expected Response |
|---|---|---|---|---|---|
| POST | `/api/auth/register` | Creates a new participant account. | Public | FirstName, LastName, Email, Password, PhoneNumber | 201 Created. 400 Bad Request for validation errors. 409 Conflict if email already exists. |
| POST | `/api/auth/login` | Authenticates a registered user. | Public | Email, Password | 200 OK with authentication information. 401 Unauthorized for invalid credentials. |
| GET | `/api/users/me` | Retrieves the profile of the currently logged-in user. | Any logged-in user | None | 200 OK with user profile. 401 Unauthorized if not authenticated. |
| PUT | `/api/users/me` | Updates the currently logged-in user's profile. | Any logged-in user | Profile fields to update | 200 OK with updated profile. 400 Bad Request for invalid data. |
| GET | `/api/events` | Retrieves upcoming RaceDay events. | Public | None | 200 OK with a list of events. |
| GET | `/api/events/{id}` | Retrieves details for a specific event. | Public | None | 200 OK with event details. 404 Not Found if the event does not exist. |
| POST | `/api/events` | Creates a new RaceDay event. | Organiser | EventName, Description, Location, EventDate, RegistrationDeadline, Status | 201 Created. 401/403 if the user is not authorised. |
| PUT | `/api/events/{id}` | Updates an existing event. | Organiser | Updated event fields | 200 OK. 404 Not Found if the event does not exist. |
| DELETE | `/api/events/{id}` | Deletes an existing event. | Organiser | None | 204 No Content. 404 Not Found if the event does not exist. |
| GET | `/api/events/{eventId}/categories` | Retrieves all categories belonging to an event. | Public | None | 200 OK with a list of categories. 404 Not Found if the event does not exist. |
| POST | `/api/events/{eventId}/categories` | Adds a category to an event. | Organiser | CategoryName, DistanceKm, EntryFee, MaximumParticipants | 201 Created. 400 Bad Request for invalid data. |
| PUT | `/api/categories/{id}` | Updates an existing event category. | Organiser | Updated category fields | 200 OK. 404 Not Found if the category does not exist. |
| DELETE | `/api/categories/{id}` | Deletes an event category. | Organiser | None | 204 No Content. 404 Not Found if the category does not exist. |
| POST | `/api/enrolments` | Enrols the logged-in participant in a selected event category. | Participant | CategoryID, EmergencyContact | 201 Created. 409 Conflict if already enrolled or capacity is reached. |
| GET | `/api/enrolments/me` | Retrieves the logged-in participant's own enrolments. | Participant | None | 200 OK with the participant's enrolments. |
| GET | `/api/events/{eventId}/enrolments` | Retrieves all enrolments for a specific event. | Organiser | None | 200 OK with event enrolments. 404 Not Found if the event does not exist. |
| DELETE | `/api/enrolments/{id}` | Cancels an enrolment belonging to the participant. | Participant | None | 204 No Content. 404 Not Found if the enrolment does not exist. |
| POST | `/api/results` | Records a participant's result. | Organiser | EnrolmentID, FinishTime, Position, ResultStatus | 201 Created. 400 Bad Request for invalid data. |
| PUT | `/api/results/{id}` | Updates a recorded race result. | Organiser | Updated result fields | 200 OK. 404 Not Found if the result does not exist. |
| GET | `/api/results/me` | Retrieves the logged-in participant's personal results. | Participant | None | 200 OK with personal results. |
| GET | `/api/events/{eventId}/results` | Retrieves results for a specific event. | Organiser | None | 200 OK with event results. 404 Not Found if the event does not exist. |
| GET | `/api/events/{eventId}/weather` | Retrieves weather information for a specific event. | Public | None | 200 OK with weather information. 404 Not Found if unavailable. |

## Role Summary

- **Public:** No authentication required.
- **Any logged-in user:** Requires a valid authenticated user.
- **Participant:** Requires the Participant role.
- **Organiser:** Requires the Organiser role.

## Notes

1. The implemented API in Part 2 should closely follow this endpoint plan.
2. Any changes made during implementation should be explained in the README.
3. Role-based access must be enforced at the API level in Part 2.
4. Request and response models may be refined during implementation while preserving the planned endpoint purpose.
