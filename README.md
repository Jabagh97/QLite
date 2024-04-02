Queue Management System (QLite)
QLite is an advanced Queue Management System designed to streamline and optimize the organization and handling of queues within local network environments. This comprehensive system ensures efficient management and coordination of tasks, providing a seamless operational experience. This repository houses all the necessary components that make up the QLite system, each meticulously designed to work in harmony.

🌟 Highlight: Dynamic Kiosk UI Designer in the Admin Portal
The Admin Portal features an innovative Designer Tool that allows users to dynamically design and customize the UI of their kiosks. This powerful tool unlocks unparalleled flexibility, enabling administrators to tailor the user interface to meet specific needs and preferences, enhancing the overall user experience.

Components Overview:
1. ASP.NET Core API
The backbone of QLite, this API facilitates communication across the system's applications. It leverages a local SQLite database for reliable CRUD operations and uses WebSocket SignalR for real-time updates and notifications.

Technologies Used:
ASP.NET Core: For API construction.
SQLite Database: Local storage solution.
WebSocket SignalR: Real-time communication.
2. Authentication Server
A secure authentication server employing OpenID and Identity Server for robust user management and security protocols, backed by a SQLite database for user data storage.

Technologies Used:
OpenID & Identity Server: Authentication and user management.
SQLite Database: Secure user information storage.
3. ASP.NET MVC Kiosk App
An intuitive user interface allowing interactions such as ticket printing and service selection, integrated with WebSocket SignalR for instant ticket update communications.

Technologies Used:
ASP.NET MVC: Application framework.
WebSocket SignalR: Syncs with the Desk App in real time.
4. ASP.NET Desk App
Designed for staff to manage the queue efficiently, featuring functionalities like ticket management and real-time updates via WebSocket SignalR.

Technologies Used:
ASP.NET MVC: Desk App development.
WebSocket SignalR: Ensures seamless system-wide communication.
5. ASP.NET MVC Admin Portal
A central hub for system administration, offering tools for system configuration, service management, and access to the dynamic Kiosk UI Designer Tool.

Technologies Used:
ASP.NET MVC: Powers the Admin Portal.
Setup Instructions:
Clone this repository to your local environment.
Follow the setup and configuration guidelines for each component as detailed in their respective documentation.
Ensure all dependencies are properly installed and configured.
Launch each application locally or deploy according to your operational needs.
Test the system's functionality by interacting with the various components to ensure everything is running smoothly.
