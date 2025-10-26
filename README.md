# Phone Book Application

## 📖 **About**
The **Phone Book Application** is a web-based system designed to manage contact information for employees within a company. This application helps to store and manage all the essential contact details for employees, including phone numbers, addresses, emails, job titles, and department information, across all company branches. It allows for easy access and updating of employee contact details in a centralized location.

This project is developed using **ASP.NET Core MVC** architecture, following the **Repository Pattern** and **Onion Architecture** to ensure a clean separation of concerns and scalability. The application is structured to be maintainable, with clear boundaries between the core logic, application services, and data access layers.

---

## 🔧 **Key Features**
- **Employee Contact Management:** Store employee details including phone numbers, emails, department, job titles, and addresses.
- **Branch Management:** Keep track of employees across multiple company branches, making it easy to access contact details for any employee, no matter the location.
- **Search & Filter:** Quickly search and filter employee information based on name, department, branch, or job title.
- **Role-Based Access Control (RBAC):** Implement security features to allow different access levels to users (admin, manager, employee) for managing contact information.
- **Multi-Branch Support:** Easily manage contacts across various branches of the organization.
- **Responsive Interface:** A clean and user-friendly interface built with **Bootstrap** to ensure accessibility on all devices.

---

## 💻 **Technologies Used**
- **Backend:** ASP.NET Core MVC
- **Frontend:** HTML, CSS, JavaScript, Bootstrap
- **Architecture:** Onion Architecture, Repository Pattern
- **Database:** SQL Server (or any other database used in your project)
- **Version Control:** Git & GitHub
- **Authentication:** ASP.NET Core Identity (if used for user roles and authentication)
- **Dependency Injection:** For managing service dependencies

---

## 🏗 **Architecture**

This project follows a layered architecture based on **Onion Architecture** and **Repository Pattern** to separate concerns and ensure scalability and maintainability.

### **Onion Architecture Layers:**

1. **Core Layer:**
   - Contains business logic and domain entities.
   - Defines interfaces and repositories that interact with the outside layers.

2. **Application Layer:**
   - Contains service classes that implement business rules and logic.
   - Coordinates the data flow from the UI to the domain and back.

3. **Infrastructure Layer:**
   - Deals with data access (using Entity Framework or any ORM).
   - Implements the repositories defined in the Core layer.

4. **Presentation Layer:**
   - The MVC controllers and views interact with the user.
   - Handles HTTP requests and responses to and from the user interface.
---

## 📬 **Contact**
- **Project Owner:** Ahmed Essam
- **Email:** [ahmedesamo778@gmail.com](mailto:ahmedesamo778@gmail.com)
