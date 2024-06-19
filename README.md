# BBShop: An E-commerce Website

BBShop is a service-oriented architecture (SOA) based e-commerce platform designed to facilitate seamless buying and selling experiences. 
It supports distinct user roles including Admin, Seller, and Buyer, each with specific functionalities to manage the online store efficiently.

# Introduction
BBShop provides a platform for small businesses to create online stores, list products, and manage orders. Buyers can browse stores, search for products, and place orders seamlessly.

# Features
- **User Registration and Authentication**: Users can register as buyers or sellers.
- **Role-Based Access**: Different functionalities for Admin, Seller, and Buyer.
- **Store and Product Management**: Sellers can create stores and add products.
- **Order Management**: Buyers can place orders, and sellers can manage and update order statuses.
- **Search and Browsing**: Enhanced search functionalities for products and stores.

## Technologies Used
- **Backend**: ASP.NET Core 7.0, Entity Framework Core 7.0
- **Frontend**: React.js
- **Database**: PostgreSQL
- **Authentication**: JWT Bearer
- **Version Control**: Git, GitHub
- **Other Libraries**: AutoMapper, Swashbuckle for API documentation


## Prerequisites
- [.NET 7.0 SDK](https://dotnet.microsoft.com/download/dotnet/7.0)
- [Node.js](https://nodejs.org/) (for frontend)
- [PostgreSQL](https://www.postgresql.org/)
- [Visual Studio](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)


## Installation

1. **Clone the repository:**
   git clone https://github.com/YourUsername/BBShop.git
   cd BBShop
1.1 Update the appsettings.json with your PostgreSQL connection string.
1.2 dotnet ef database update

Navigate to the frontend directory
npm install

Make Sure to run the backend project first then frontend
npm start
