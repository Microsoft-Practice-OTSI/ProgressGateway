# ProgressGateway

ProgressGateway is a reusable real-time progress notification solution built using
ASP.NET Core 8 and SignalR.

The solution provides a generic mechanism for monitoring long-running business
processes and displaying their progress to users in real time.

Examples of processes that can use the gateway include:

- Work Order Creation
- Employee Onboarding
- File Processing
- Report Generation
- Document Processing
- Data Import / Export
- Batch Processing
- Other long-running business workflows

---

# 1. Project Purpose

The purpose of ProgressGateway is to provide a common and reusable solution for
displaying real-time progress updates for long-running processes.

Instead of implementing SignalR communication and progress tracking separately
for every business process, applications can use the same ProgressGateway API
and SignalR infrastructure.

The gateway provides information such as:

- Execution ID
- Current step
- Step status
- Progress percentage
- Progress message
- Completion status
- Failure status

For example:

```text
Execution ID
      |
      v
Validate Request       25%
      |
      v
Process Data           50%
      |
      v
Generate Document      75%
      |
      v
Complete               100%