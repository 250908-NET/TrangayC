# Minimal Console ToDo App

A simple, clean console-based ToDo application demonstrating the use of a small service layer and model. 

## Features

- Add a new ToDo item
- List all ToDo items with status and created date
- Mark an item as complete
- Delete an item
- Simple in-memory storage using a service (`ToDoService`)

## Tech Stack

- .NET 9 (C#)
- Implicit usings and nullable enabled (see the project `.csproj` file)

## Project Structure

```
minimal_console_todo_app/
├─ Program.cs                 // Console UI loop and interaction
├─ services/
│  ├─ IToDoService.cs         // Service interface
│  └─ ToDoService.cs          // In-memory implementation
├─ model/
│  └─ ToDoItem.cs             // Data model
└─ <project-name>.csproj      // Project configuration
```

## Getting Started

### Prerequisites

- .NET SDK 9.0 or newer installed
  - Verify by running: `dotnet --version`

### Build

- From the project directory:

```bash
dotnet build
```

### Run

- From the project directory:

```bash
dotnet run
```

## Usage

After starting the app, you’ll see a menu like this:

```
==== Todo App ====
1. Add item
2. List items
3. Mark item complete
4. Delete item
5. Exit
Choose an option:
```

- Select an option by entering its number and pressing Enter.
- For options 3 and 4, you can first list items to see the IDs, then enter the ID to complete or delete.

## Design Notes

- The service (`services/ToDoService.cs`) handles all operations on an in-memory `List<ToDoItem>`.
- IDs are generated to be unique and increasing based on the current max ID.
- `listItems()` returns a formatted string for display. It currently uses `StringBuilder` for clarity and efficiency when building multi-line output. For small lists, alternatives like `string.Join` over a `List<string>` would also be fine.
- With `<ImplicitUsings>enable</ImplicitUsings>` in the project `.csproj`, common namespaces like `System`, `System.Linq`, and `System.Collections.Generic` are available without explicit `using` directives. `System.Text` remains explicitly imported for `StringBuilder`.

## Troubleshooting

- If you encounter build issues, ensure your .NET SDK is up to date and the project directory is the working directory when running commands.
- If the console doesn’t refresh as expected, your terminal may cache output—press Enter to continue or restart the app.

## License

No license. This was an assignemnt.
