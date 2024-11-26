# Collection Manager

Welcome to **Collection Manager**! This is a dynamic web app designed for anyone who wants a centralized place to store, organize, and customize their collections. Built with ASP.NET Core MVC and PostgreSQL, Collection Manager offers a range of features to make managing collections easy and engaging.

You can try out the live demo here: [Collection Manager Live Demo](http://users.somee.com/).

## 🚀 Features

Here’s a peek at what **Collection Manager** can do:

- **Organize Your Collections**: Create and store collections with customizable settings. Perfect for enthusiasts, collectors, or anyone looking to keep things organized!
- **Interactive Community**: 
  - **Likes and Comments**: Users can like and comment on collections, fostering community interaction.
  - **Multilingual Support**: The app is fully localized, making it accessible to a global audience.
  - **Full-Text Search**: Quickly find what you’re looking for with robust search functionality.
- **Admin Panel**: Manage users and roles with comprehensive access control. It’s all set up for easy moderation and control over your platform.
- **Jira & Salesforce Integrations**:
  - **Jira Ticketing**: Track and manage tasks seamlessly with basic Jira ticket integration.
  - **Salesforce**: Connect user data from Salesforce for enhanced collaboration and a better CRM experience.

## 🛠 Tech Stack

Here’s what powers **Collection Manager**:

- **Backend**: ASP.NET Core MVC, Entity Framework Core
- **Database**: PostgreSQL for efficient and reliable data storage
- **Real-Time Updates**: SignalR for live updates and notifications
- **External Integrations**:
  - **Jira API** for task tracking
  - **Salesforce API** for user data and CRM capabilities

## 🚀 Getting Started

Want to run Collection Manager on your own machine? Here’s how:

1. **Clone the repository**:
   ```bash
   git clone https://github.com/wajiul/collectionmanager.git
   cd collection-manager
2. **Set up the database**:
  - Make sure you have PostgreSQL installed.
  - Create a new database and update the connection string in the configuration file.
3. **Install dependencies**:
   ```bash
   dotnet restore
4. **Set up your API keys**:
  - Get API credentials from Jira and Salesforce.
  - Add these to your environment variables or directly in appsettings.json.

5. **Run migrations**:
   ```bash
   dotnet ef database update
6. **Start the application**:
   ```bash
   dotnet run
