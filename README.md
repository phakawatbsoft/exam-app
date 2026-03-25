# exam-app

ระบบสอบออนไลน์ (Online Examination System) แบบ Full-Stack พัฒนาด้วย Angular 21 + ASP.NET Core 10.0 รองรับการแสดงคำถามปรนัย รับคำตอบ และคำนวณคะแนนอัตโนมัติ

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | Angular 21, PrimeNG, Tailwind CSS v4, SSR |
| Backend | ASP.NET Core 10.0 (C#) |
| Database | Entity Framework Core (In-Memory / SQL Server) |
| API Docs | NSwag / OpenAPI |

## โครงสร้างโปรเจกต์

```
exam-app/
├── exam-app.sln
├── backend/
│   ├── Controllers/
│   │   ├── QuestionsController.cs   # CRUD สำหรับจัดการคำถาม
│   │   └── ResultsController.cs     # รับคำตอบและคำนวณคะแนน
│   ├── Models/
│   │   ├── Question.cs              # โมเดลคำถาม
│   │   ├── ExamResult.cs            # โมเดลผลการสอบ
│   │   └── QuizContext.cs           # Entity Framework DbContext
│   ├── Program.cs
│   ├── appsettings.json
│   └── backend.csproj
└── frontend/
    ├── src/
    │   ├── app/
    │   │   ├── app.ts               # Root component (UI หลัก)
    │   │   ├── app.routes.ts        # Client routing
    │   │   └── services/
    │   │       └── quiz.ts          # HTTP service เรียก API
    │   ├── server.ts                # Express server (SSR)
    │   └── styles.css               # Global styles (Tailwind)
    ├── angular.json
    └── package.json
```

## การติดตั้งและรัน

### ข้อกำหนดเบื้องต้น

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js 20+](https://nodejs.org/)

### รัน Backend

```bash
cd backend
dotnet restore
dotnet run
```

แอปจะรันที่:
- HTTP: `http://localhost:5076`
- HTTPS: `https://localhost:7270`

### รัน Frontend

```bash
cd frontend
npm install
npm start
```

Frontend จะรันที่ `http://localhost:4200`

### Build สำหรับ Production

```bash
# Backend
cd backend
dotnet publish -c Release

# Frontend
cd frontend
ng build                       # SSR build → dist/
npm run serve:ssr:frontend     # รัน SSR ที่ port 4000
```

## ฟีเจอร์

- กรอกชื่อ-นามสกุลก่อนเริ่มสอบ
- แสดงคำถามปรนัยพร้อม radio button
- ส่งคำตอบและรับคะแนนอัตโนมัติ
- แสดงผลคะแนน เช่น `8 / 10`
- ทำข้อสอบใหม่ได้โดยกดปุ่ม Retake

## API Endpoints

### Questions `/api/questions`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/questions` | ดึงคำถามทั้งหมด |
| GET | `/api/questions/{id}` | ดึงคำถามตาม ID |
| POST | `/api/questions` | เพิ่มคำถามใหม่ |
| PUT | `/api/questions/{id}` | แก้ไขคำถาม |
| DELETE | `/api/questions/{id}` | ลบคำถาม |

### Results `/api/results`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/results` | ดึงผลการสอบทั้งหมด |
| GET | `/api/results/{id}` | ดึงผลการสอบตาม ID |
| POST | `/api/results` | ส่งคำตอบและรับคะแนน |
| PUT | `/api/results/{id}` | แก้ไขผลการสอบ |
| DELETE | `/api/results/{id}` | ลบผลการสอบ |

### ตัวอย่าง Request — ส่งคำตอบ

```http
POST /api/results
Content-Type: application/json

{
  "fullName": "สมชาย ใจดี",
  "answers": "{\"1\": \"คำตอบที่ 1\", \"2\": \"คำตอบที่ 2\", \"3\": \"คำตอบที่ 3\"}"
}
```

Response จะมีคะแนนที่คำนวณอัตโนมัติโดยเปรียบเทียบกับ `CorrectAnswer` ของแต่ละคำถาม

## Data Models

### Question

```json
{
  "id": 1,
  "questionText": "คำถามที่ 1",
  "options": "คำตอบที่ 1,คำตอบที่ 2,คำตอบที่ 3,คำตอบที่ 4",
  "correctAnswer": "คำตอบที่ 1"
}
```

### ExamResult

```json
{
  "id": 1,
  "fullName": "สมชาย ใจดี",
  "score": 3,
  "answers": "{\"1\": \"คำตอบที่ 1\", \"2\": \"คำตอบที่ 2\", \"3\": \"คำตอบที่ 3\"}"
}
```

## API Documentation

เมื่อรันในโหมด Development สามารถดู OpenAPI spec ได้ที่:

```
http://localhost:5076/openapi/v1.json
```

## หมายเหตุ

- ฐานข้อมูลเป็นแบบ In-Memory — ข้อมูลจะหายเมื่อรีสตาร์ทแอป
- เมื่อเริ่มต้นแอป ระบบจะสร้างคำถามตัวอย่าง 3 ข้อให้อัตโนมัติ
- รองรับการเชื่อมต่อ SQL Server โดยแก้ไข connection string ใน `appsettings.json`
- Frontend เรียก API ที่ `http://localhost:5076/api` โดยตรง (แก้ได้ใน `frontend/src/app/services/quiz.ts`)
