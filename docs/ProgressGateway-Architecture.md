# ProgressGateway - Architecture and Technical Design

## 1. Introduction

ProgressGateway is a reusable real-time progress notification solution built
using **ASP.NET Core 8** and **SignalR**.

The solution provides a generic mechanism for monitoring long-running business
processes and displaying their progress, status, percentage, and messages to
users in real time.

The primary objective of the POC is to separate business-process execution
from real-time progress notification so that multiple business processes can
use the same progress infrastructure.

Example business processes include:

- Work Order Creation
- Employee Onboarding
- File Processing
- Report Generation
- Document Processing
- Data Import / Export
- Batch Processing
- Other long-running business workflows

---

## 2. Objectives

The main objectives of ProgressGateway are:

1. Provide a reusable real-time progress notification mechanism.
2. Avoid implementing SignalR progress communication separately for each
   business process.
3. Provide a generic REST API for publishing progress updates.
4. Use SignalR to deliver progress updates to the appropriate browser.
5. Associate progress updates with a unique execution using `ExecutionId`.
6. Display progress percentage, current step, status, and message in the UI.
7. Keep business-specific processing logic outside the generic gateway.
8. Provide an architecture that can be extended for future production use.

---

## 3. Solution Overview

The solution contains two applications.

### 3.1 ProgressGateway.Api

`ProgressGateway.Api` is the central progress notification API.

Responsibilities:

- Receive progress updates through REST APIs.
- Receive process completion notifications.
- Host the SignalR `ProgressHub`.
- Manage SignalR execution groups.
- Broadcast progress updates to connected clients.

The API is intentionally generic and does not contain business-specific
processing logic.

### 3.2 ProgressGateway.UI

`ProgressGateway.UI` is an ASP.NET Core MVC application using Razor Views.

It demonstrates how multiple business processes can use the same
ProgressGateway infrastructure.

The current UI demonstrates:

- Work Order
- Employee Onboarding
- File Processing
- Report Generation

Each business process has its own controller and helper/business logic while
sharing the common `ProgressGatewayClient` and SignalR infrastructure.

---

## 4. High-Level Architecture

```text
                         +----------------------+
                         |       Browser        |
                         |                      |
                         |     Razor View       |
                         |     Progress Bar     |
                         |     Current Step     |
                         |     Status           |
                         |     Message          |
                         +----------+-----------+
                                    |
                                    | SignalR
                                    |
                                    v
                         +----------------------+
                         | ProgressGateway.Api  |
                         |                      |
                         |    ProgressHub       |
                         |                      |
                         |  Generic Progress    |
                         |       API            |
                         +----------+-----------+
                                    ^
                                    |
                                    | HTTP
                                    |
                         +----------+-----------+
                         |   ProgressGateway.UI |
                         |                      |
                         | Controller           |
                         |       |              |
                         |       v              |
                         | Helper / Business    |
                         | Logic                |
                         |       |              |
                         |       v              |
                         | ProgressGatewayClient|
                         +----------------------+
```

### Communication

There are two communication paths:

1. **HTTP** - used by the business logic/client to send progress information
   to `ProgressGateway.Api`.
2. **SignalR** - used by `ProgressGateway.Api` to broadcast progress
   notifications to the browser in real time.

---

## 5. Component Architecture

### 5.1 Browser / Razor View

The browser is responsible for:

- Displaying the process UI.
- Generating an `ExecutionId`.
- Establishing the SignalR connection.
- Joining the appropriate execution group.
- Starting the business process through the UI controller.
- Receiving progress events.
- Updating the progress bar.
- Updating step status.
- Displaying progress messages.
- Displaying failures.

### 5.2 UI Controller

The controller is responsible for:

- Receiving the Start request from the Razor View.
- Validating the request.
- Starting the appropriate business helper/process.
- Returning an immediate response to the browser.

The controller should not contain the complete business workflow.

### 5.3 Business Helper

The helper contains the business-process-specific workflow.

For example:

```text
WorkOrderHelper
FileProcessingHelper
EmployeeOnboardingHelper
ReportGenerationHelper
```

The helper:

- Executes business steps.
- Determines the progress percentage.
- Determines the step status.
- Sends progress updates using `ProgressGatewayClient`.
- Handles process-specific exceptions.

### 5.4 ProgressGatewayClient

`ProgressGatewayClient` is the UI-side client responsible for communicating
with `ProgressGateway.Api`.

It abstracts the HTTP communication so business helpers do not need to
implement HTTP request handling repeatedly.

### 5.5 ProgressGateway.Api

The API receives progress information and publishes it through SignalR.

It acts as the centralized progress notification gateway.

### 5.6 ProgressHub

`ProgressHub` is the SignalR hub.

The browser joins an execution-specific group using:

```csharp
JoinExecutionGroup(string executionId)
```

This allows progress updates for one execution to be sent only to the
clients associated with that execution.

---

## 6. End-to-End Process Flow

The typical process flow is:

```text
User
 |
 | Click Start
 v
Razor View
 |
 | Generate ExecutionId
 v
SignalR Connection
 |
 | JoinExecutionGroup(ExecutionId)
 v
UI Controller
 |
 | Start process
 v
Business Helper
 |
 +--> Step 1
 |       |
 |       +--> ProgressGatewayClient
 |
 +--> Step 2
 |       |
 |       +--> ProgressGatewayClient
 |
 +--> Step 3
 |       |
 |       +--> ProgressGatewayClient
 |
 +--> Step 4
         |
         +--> ProgressGatewayClient
                    |
                    | HTTP
                    v
           ProgressGateway.Api
                    |
                    v
              ProgressHub
                    |
                    | SignalR
                    v
                 Browser
                    |
                    v
             Update Progress UI
```

---

## 7. Start Button Flow

When the user clicks the Start button:

1. The progress area becomes visible.
2. The UI generates a unique `ExecutionId`.
3. The browser establishes the SignalR connection.
4. The browser joins the execution group.
5. The UI calls the corresponding controller Start action.
6. The controller starts the corresponding helper.
7. The helper executes the business process.
8. Each step sends progress information to the gateway.
9. The API broadcasts the progress update using SignalR.
10. The browser receives the update.
11. The progress bar is updated.
12. The step status and message are updated.
13. The process continues until completion or failure.

Joining the execution group before starting the business process is important
because it helps ensure that the browser does not miss the initial progress
updates.

---

## 8. Execution ID and SignalR Groups

Each process execution receives a unique `ExecutionId`.

Example:

```text
e1f550ee-ae63-4946-a809-ceed3b658819
```

The browser uses the execution ID to join a SignalR group:

```csharp
await connection.invoke(
    "JoinExecutionGroup",
    executionId
);
```

The hub adds the browser connection to the group:

```csharp
await Groups.AddToGroupAsync(
    Context.ConnectionId,
    executionId
);
```

The execution ID therefore provides the association between:

```text
Business Process Execution
          |
          v
      ExecutionId
          |
          v
    SignalR Group
          |
          v
      Browser Client
```

This prevents progress notifications from unrelated executions from being
displayed in the wrong browser session.

---

## 9. SignalR Architecture

The current `ProgressHub` exposes methods for joining and leaving an execution
group.

### Join Execution Group

```csharp
public async Task JoinExecutionGroup(string executionId)
{
    await Groups.AddToGroupAsync(
        Context.ConnectionId,
        executionId);
}
```

### Leave Execution Group

```csharp
public async Task LeaveExecutionGroup(string executionId)
{
    await Groups.RemoveFromGroupAsync(
        Context.ConnectionId,
        executionId);
}
```

The browser uses the SignalR connection to receive progress notifications.

SignalR provides real-time communication without requiring the browser to
continuously poll the API for progress.

---

## 10. Progress Update Model

A progress update contains information similar to:

```json
{
  "executionId": "e1f550ee-ae63-4946-a809-ceed3b658819",
  "step": "ProcessFile",
  "status": "InProgress",
  "percentage": 50,
  "message": "File processing is in progress..."
}
```

The progress information can include:

- `ExecutionId`
- `Step`
- `Status`
- `Percentage`
- `Message`

### Supported Statuses

```text
Pending
InProgress
Completed
Failed
```

---

## 11. Progress Percentage

The business helper determines the progress percentage based on the process
steps.

Example for File Processing:

```text
Validate File
      |
      | Completed
      v
     25%
      |
      v
Upload File
      |
      | Completed
      v
     50%
      |
      v
Process File
      |
      | Completed
      v
     75%
      |
      v
Save Result
      |
      | Completed
      v
    100%
```

The gateway does not need to understand the meaning of the percentage. It
simply transports the progress information.

---

## 12. Business Process Architecture

Each business process follows the same general structure.

### Work Order

```text
WorkOrderController
        |
        v
WorkOrderHelper
        |
        v
ProgressGatewayClient
        |
        v
ProgressGateway.Api
        |
        v
ProgressHub
        |
        v
Browser
```

### File Processing

```text
FileProcessingController
        |
        v
FileProcessingHelper
        |
        v
ProgressGatewayClient
        |
        v
ProgressGateway.Api
        |
        v
ProgressHub
        |
        v
Browser
```

### Report Generation

```text
ReportGenerationController
        |
        v
ReportGenerationHelper
        |
        v
ProgressGatewayClient
        |
        v
ProgressGateway.Api
        |
        v
ProgressHub
        |
        v
Browser
```

### Employee Onboarding

```text
EmployeeOnboardingController
        |
        v
EmployeeOnboardingHelper
        |
        v
ProgressGatewayClient
        |
        v
ProgressGateway.Api
        |
        v
ProgressHub
        |
        v
Browser
```

The common gateway infrastructure remains unchanged when a new business
process is added.

---

## 13. Example File Processing Flow

The File Processing sample contains four steps:

```text
Start
 |
 +--> Validate File
 |       |
 |       +--> InProgress - 0%
 |       |
 |       +--> Completed - 25%
 |
 +--> Upload File
 |       |
 |       +--> InProgress - 25%
 |       |
 |       +--> Completed - 50%
 |
 +--> Process File
 |       |
 |       +--> InProgress - 50%
 |       |
 |       +--> Completed - 75%
 |
 +--> Save Result
         |
         +--> InProgress - 75%
         |
         +--> Completed - 100%
```

---

## 14. Example Work Order Flow

The Work Order sample contains steps such as:

```text
Pre Validation
      |
      v
Create Work Order
      |
      v
Create Documents
      |
      v
Create Confirmations
      |
      v
Generate Invoice
      |
      v
Completed
```

Each step reports its status and progress to the gateway.

---

## 15. API Architecture

The API provides generic progress operations.

Example operations include:

```text
POST /api/Progress/update
POST /api/Progress/complete
```

### Update Progress

The update operation is used when a process starts or completes a step.

Conceptually:

```text
Business Helper
      |
      | Progress Update
      v
ProgressGatewayClient
      |
      | HTTP POST
      v
ProgressController
      |
      v
ProgressService
      |
      v
SignalR ProgressHub
      |
      v
Execution Group
```

### Complete Process

The completion operation is used when the complete business process has
finished successfully.

The completion notification can be used by the UI to perform any final
processing, such as displaying the completed state or navigating to a result
page.

---

## 16. UI Architecture

The UI follows ASP.NET Core MVC with Razor Views.

```text
Razor View
    |
    v
Controller
    |
    v
Helper
    |
    v
ProgressGatewayClient
```

The Razor View is responsible for presentation and user interaction.

The controller handles the HTTP request.

The helper contains the process workflow.

The common client handles communication with the ProgressGateway API.

---

## 17. UI Progress Display

The progress UI provides:

- Progress bar
- Percentage
- Current step
- Step status
- Progress message
- Completed state
- Failed state

The progress section can remain hidden initially and become visible when the
user starts a process.

Example:

```text
Before Start:

[ Start ]

After Start:

[████████████░░░░░░░░] 60%

Process File
    ✓ Validate File       Completed
    ✓ Upload File         Completed
    ● Process File        In Progress...
    ○ Save Result         Pending

Message:
File processing is in progress...
```

---

## 18. Technologies Used

| Technology | Purpose |
|---|---|
| .NET 8 | Application platform |
| ASP.NET Core | API and web application framework |
| ASP.NET Core MVC | UI architecture |
| Razor Views | Server-rendered UI |
| SignalR | Real-time browser notifications |
| REST API | Progress communication |
| C# | Application and business logic |
| JavaScript | SignalR client and UI updates |
| Bootstrap | UI styling |

---

## 19. Project Structure

```text
ProgressGateway
│
├── README.md
├── .gitignore
│
├── docs
│   └── ProgressGateway-Architecture.md
│
├── ProgressGateway.Api
│   │
│   ├── Controllers
│   │   └── ProgressController.cs
│   │
│   ├── Hubs
│   │   └── ProgressHub.cs
│   │
│   ├── Models
│   │
│   ├── Services
│   │
│   ├── Program.cs
│   ├── appsettings.json
│   └── ProgressGateway.Api.csproj
│
└── ProgressGateway.UI
    │
    ├── Controllers
    │   ├── WorkOrderController.cs
    │   ├── FileProcessingController.cs
    │   ├── EmployeeOnboardingController.cs
    │   └── ReportGenerationController.cs
    │
    ├── Helpers
    │   ├── WorkOrderHelper.cs
    │   ├── FileProcessingHelper.cs
    │   ├── EmployeeOnboardingHelper.cs
    │   └── ReportGenerationHelper.cs
    │
    ├── Models
    │
    ├── Services
    │   └── ProgressGatewayClient.cs
    │
    ├── Views
    │
    ├── wwwroot
    │
    ├── Program.cs
    ├── appsettings.json
    └── ProgressGateway.UI.csproj
```

---

## 20. Configuration

### ProgressGateway.Api

API configuration is located in:

```text
ProgressGateway.Api/appsettings.json
```

The API hosts the SignalR hub at:

```text
/progressHub
```

The actual API URL depends on the local launch configuration.

### ProgressGateway.UI

UI configuration is located in:

```text
ProgressGateway.UI/appsettings.json
```

The UI requires the ProgressGateway API base URL.

Example:

```json
{
  "ProgressGateway": {
    "ApiBaseUrl": "https://localhost:7007"
  }
}
```

The URL should be updated to match the API URL configured in the local
environment.

---

## 21. Setup and Run

### Prerequisites

- .NET 8 SDK
- Visual Studio 2022 or later
- Git
- Modern web browser

Verify .NET:

```bash
dotnet --version
```

### Clone

```bash
git clone https://github.com/Microsoft-Practice-OTSI/ProgressGateway.git
cd ProgressGateway
```

### Restore

```bash
dotnet restore
```

### Start API

```bash
dotnet run --project ProgressGateway.Api
```

Verify Swagger using the configured API URL:

```text
https://localhost:<api-port>/swagger
```

### Start UI

```bash
dotnet run --project ProgressGateway.UI
```

Open the UI using the configured UI URL.

Both the API and UI should be running for the complete real-time progress flow.

---

## 22. Error Handling

If a process step fails, the helper can send a failed progress update.

Example:

```text
Step: ProcessFile
Status: Failed
Percentage: 50
Message: File processing failed.
```

The UI displays the failed state to the user.

The current POC demonstrates basic error propagation.

Production implementation should add centralized logging, persistent error
information, retry handling, and recovery mechanisms.

---

## 23. Current Implementation

The current POC demonstrates:

- Generic progress REST API
- SignalR Progress Hub
- SignalR execution groups
- ASP.NET Core MVC UI
- Razor Views
- Real-time progress updates
- Progress bar
- Progress percentage
- Step status
- Progress messages
- Work Order sample
- Employee Onboarding sample
- File Processing sample
- Report Generation sample
- Common `ProgressGatewayClient`
- Business-specific helpers

---

## 24. Current Limitations

This implementation is currently a **Proof of Concept (POC)**.

### Database Persistence

Progress information is not currently persisted in a database.

If the browser refreshes or disconnects, previously displayed progress information
may not be available.

### Background Processing

The current sample uses asynchronous task execution for demonstration purposes.

A production implementation should use a dedicated background processing
mechanism.

### Authentication

Authentication and authorization are not currently implemented.

The POC does not currently use JWT authentication.

### Production Resilience

Production-level retry, recovery, monitoring, and distributed processing are
not yet implemented.

---

## 25. Current Architecture vs Future Architecture

### Current POC Architecture

```text
Business Process
      |
      v
Controller
      |
      v
Helper
      |
      v
ProgressGatewayClient
      |
      | HTTP
      v
ProgressGateway.Api
      |
      v
ProgressHub
      |
      | SignalR
      v
Browser
```

### Potential Production Architecture

```text
Business Application
        |
        v
Controller / API
        |
        v
Background Job
        |
        v
Message Queue
        |
        v
Worker
        |
        v
ProgressGateway
        |
        v
SignalR
        |
        v
Browser
```

The production architecture should be finalized based on deployment,
scalability, reliability, and organizational requirements.

---

## 26. Security Considerations

Before production deployment, the following security controls should be
implemented:

- Authentication
- Authorization
- HTTPS
- Restricted CORS configuration
- Secure SignalR connections
- Input validation
- Execution ID validation
- Rate limiting
- Secure secret management
- Environment-specific configuration

Sensitive information such as the following must not be committed to source
control:

- Passwords
- API keys
- Access tokens
- Database credentials
- Private keys
- Connection strings containing credentials

Use environment variables, secret stores, or appropriate configuration
management mechanisms for sensitive values.

---

## 27. Scalability Considerations

The current implementation is intended for POC validation.

For a production deployment with multiple API instances, SignalR scale-out
should be evaluated.

Potential options include:

- Azure SignalR Service
- Redis backplane
- Other distributed SignalR approaches

The appropriate approach depends on the deployment environment and expected
scale.

---

## 28. Future Scope / Next Steps

The following enhancements are planned or can be considered for future
versions.

### 28.1 JWT Authentication and Authorization

Introduce authentication and authorization for:

- Progress REST APIs
- SignalR connections
- Progress publishers
- Progress consumers

### 28.2 Database Persistence

Persist execution and progress information, including:

```text
ExecutionId
ProcessName
Step
Status
Percentage
Message
StartedOn
CompletedOn
ErrorMessage
```

This would allow the UI to recover the latest execution state after a refresh
or reconnection.

### 28.3 Production Background Processing

Replace demonstration task execution with a production-grade background
processing architecture.

Potential technologies include:

- Hangfire
- .NET Worker Service
- Azure Service Bus
- RabbitMQ
- Azure Functions

### 28.4 Reusable SignalR Client

Create a reusable client-side component to centralize:

- SignalR connection
- Automatic reconnection
- Execution groups
- Progress updates
- Progress percentage
- Step status
- Error handling

### 28.5 .NET / NuGet Client Package

Create a reusable package such as:

```text
ProgressGateway.Client
```

This would allow .NET applications to integrate with ProgressGateway without
duplicating HTTP client implementation.

### 28.6 Angular Integration

Provide an Angular service or library that can consume the same ProgressGateway
infrastructure.

### 28.7 Docker / Containerization

Containerize the API and UI applications for deployment to platforms such as:

- Docker
- Kubernetes
- Azure Container Apps

### 28.8 Centralized Logging and Monitoring

Introduce production-grade logging and monitoring using technologies such as:

- `ILogger`
- Serilog
- Application Insights

Monitor:

- API failures
- SignalR connections
- Failed executions
- Process duration
- Background processing failures
- Active executions

### 28.9 Health Checks

Add health checks for:

- API availability
- Database connectivity
- Background processing infrastructure
- External dependencies

### 28.10 Retry and Recovery

Introduce retry and recovery mechanisms for:

- Temporary API failures
- SignalR reconnection
- Background processing failures
- External service failures

### 28.11 Performance and Scalability Testing

Conduct:

- Load testing
- SignalR connection testing
- Concurrent execution testing
- API performance testing
- Resource utilization testing

---

## 29. Development Guidelines

When adding a new business process, reuse the existing ProgressGateway
infrastructure.

For example, a new `DataImport` process can follow:

```text
DataImportController
        |
        v
DataImportHelper
        |
        v
ProgressGatewayClient
        |
        v
ProgressGateway.Api
        |
        v
ProgressHub
        |
        v
Browser
```

The new process should not require changes to the generic progress
notification infrastructure unless a new generic capability is required.

The business process should be responsible for:

- Business rules
- Business workflow
- Process steps
- Progress percentage
- Business-specific error handling

ProgressGateway should remain responsible for:

- Progress transport
- Real-time notification
- SignalR execution groups

---

## 30. Production Readiness

The current solution should be considered a **POC implementation** and not a
production-ready distributed processing platform.

Before production deployment, the following areas should be addressed:

- Authentication and authorization
- Persistent execution state
- Production background processing
- Distributed SignalR configuration where required
- Centralized logging
- Monitoring
- Health checks
- Retry and recovery
- Secure configuration
- Secret management
- CORS restrictions
- Scalability testing
- Performance testing

---

## 31. Conclusion

ProgressGateway demonstrates a reusable architecture for providing real-time
progress updates for long-running business processes.

The solution separates:

```text
Business Process
       |
       v
Progress Notification
       |
       v
Real-Time UI Updates
```

This separation allows multiple business applications and workflows to use a
common progress notification infrastructure while keeping business-specific
logic outside the gateway.

The current POC validates the core real-time progress architecture and provides
a foundation for future production enhancements such as authentication,
persistence, background processing, scalability, monitoring, and resilience.
