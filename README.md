Queue Management System (QLite)

This system is designed to manage queues within a local network environment, providing efficient organization and handling of various tasks. Below, you'll find a detailed overview of the components included in this repository.

Components
1. ASP.NET Core API
The core component of this system is the ASP.NET Core API. It serves as the communication hub for the various applications within the system. The API utilizes a local portable SQLite database to handle CRUD operations, ensuring data integrity and reliability. Additionally, it manages WebSocket SignalR communication between the applications, facilitating real-time updates and notifications.

Technical Details:

ASP.NET Core: Utilized for building the API.
SQLite Database: Used as the local database for storing data.
WebSocket SignalR: Employed for establishing real-time communication between applications.

2. Authentication Server
To ensure secure access to the system, an authentication server is implemented using OpenID and Identity Server. This server handles user authentication, login, logout, and user management functionalities. Similar to the API, it also utilizes a local SQLite database for storing user information securely.

Technical Details:

OpenID: Used for authentication and authorization.
Identity Server: Implemented for user management
and authentication.
SQLite Database: Employed to store user information securely.


3. ASP.NET MVC Kiosk App
The Kiosk App provides an intuitive interface for users to interact with the system. Users can print tickets, select services, and perform other actions. This application communicates with the Desk App via WebSocket SignalR to inform about newly created tickets, ensuring seamless coordination between different parts of the system.

Technical Details:

ASP.NET MVC: Utilized to build the Kiosk App.
WebSocket SignalR: Used for real-time communication with the Desk App.
4. ASP.NET Desk App
The Desk App serves as the interface for staff members to manage the queue efficiently. Staff can view waiting tickets, call, park, transfer tickets, leave notes, and perform other related actions. Each staff member is assigned a unique desk ID and number, which are determined by the admin during the setup process. Similar to other components, the Desk App communicates with other parts of the system via WebSocket SignalR.

Technical Details:

ASP.NET MVC: Utilized to build the Desk App.
WebSocket SignalR: Used for real-time communication with other components.

5. ASP.NET MVC Admin Portal
The Admin Portal provides administrators with a centralized interface to manage the entire system. Administrators can configure settings, define services, manage ticket pools, and perform other administrative tasks. This portal ensures easy administration and customization of the system.

Technical Details:

ASP.NET MVC: Utilized to build the Admin Portal.
Setup
To set up the Queue Management System, follow these steps:

Clone this repository to your local machine.
Set up and configure each component according to the provided documentation.
Ensure that all dependencies are installed and configured properly.
Run each application locally or deploy them to your desired environment.
Verify the functionality of the system by interacting with the various applications.
