# API description

| API 	| Description 	 | Request body 	     | Response body |
|-------|----------------|---------------------|---------------|
| GET /api/question | Returns list questions | None | Json file with list of Question objects |
| GET /api/question/{userId}/{questionId?} | If UserId <= 0 returns 400 Bad Request, if questionId != null => returns question with another id  | None | JSON with Question object |
|       |                |                     |               |

<img src="https://i.ibb.co/HtHygkD/Question-API-implementation.jpg" alt="Question-API-implementation" border="0">
