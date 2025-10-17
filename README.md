# 🚀 GitHub Profile Viewer

A modern, responsive web application built with **Blazor Server** that allows users to explore GitHub profiles and repositories with detailed insights, statistics, and visualizations.

🌐 **[Live Demo](https://gpv.sametcc.me)** - Try it now!

![.NET](https://img.shields.io/badge/.NET-9.0-purple?style=for-the-badge&logo=dotnet)
![Bulma](https://img.shields.io/badge/Bulma-CSS-00D1B2?style=for-the-badge&logo=bulma)
![Blazor](https://img.shields.io/badge/Blazor-Server-blue?style=for-the-badge&logo=blazor)
![Docker](https://img.shields.io/badge/Docker-Ready-blue?style=for-the-badge&logo=docker)

## ✨ Features

### 🔍 Profile Exploration

- **User Search**: Search for GitHub users with real-time suggestions
- **Comprehensive Profile View**: Display user information, statistics, and bio
- **Repository Analysis**: Browse and analyze user repositories with detailed metrics
- **Gist Support**: View and explore user's gists

### 📊 Data Visualization

- **Interactive Charts**: Visual representation of repository statistics
- **Language Distribution**: See programming language usage across repositories
- **Activity Insights**: Track user activity and contributions

### 🔐 Authentication & Rate Limiting

- **GitHub Token Integration**: Optional personal access token for enhanced API limits
- **Rate Limit Monitoring**: Real-time display of API quota usage
- **Secure Storage**: Tokens stored securely in browser's local storage

### 🎨 User Experience

- **Responsive Design**: Built with Bulma CSS framework for mobile-first experience
- **Fast Performance**: Server-side rendering with interactive components
- **Caching System**: Intelligent caching to reduce API calls and improve performance

### 🛠️ Technical Features

- **Modern Architecture**: Clean separation of concerns with services and interfaces
- **Error Handling**: Comprehensive error handling and user feedback
- **Docker Support**: Ready for containerized deployment
- **Memory Caching**: Efficient caching system to optimize API usage

## 🚀 Quick Start

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Git](https://git-scm.com/)
- (Optional) [Docker](https://www.docker.com/) for containerized deployment

### Installation

1. **Clone the repository**

   ```bash
   git clone https://github.com/sametcn99/GPVBlazor.git
   cd GPVBlazor
   ```

2. **Navigate to the project directory**

   ```bash
   cd GPVBlazor/GPVBlazor
   ```

3. **Restore dependencies**

   ```bash
   dotnet restore
   ```

4. **Run the application**

   ```bash
   dotnet run
   ```

## 🐳 Docker Deployment

### Build and run with Docker

```bash
# Build the Docker image
docker build -t gpvblazor .

# Run the container
docker run -d -p 8080:8080 --name gpvblazor-app gpvblazor
```

## 🔧 Configuration

### Application Settings

The application can be configured through `appsettings.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### GitHub Token Setup

1. **Generate a Personal Access Token**:

   - Go to GitHub Settings → Developer settings → Personal access tokens
   - Generate a new token with appropriate permissions
   - Copy the token

2. **Add Token in Application**:

   - Enter the token in the authentication section on the home page
   - The token will be stored securely in your browser's local storage

## 🏗️ Project Structure

```text
GPVBlazor/
├── Components/                  # Blazor components
│   ├── Displays/               # Display components
│   │   ├── AuthQuotaDisplay.razor
│   │   ├── UserProfileDisplay.razor
│   │   └── ...
│   ├── Layout/                 # Layout components
│   └── Pages/                  # Page components
├── Services/                   # Business logic services
│   ├── Interfaces/            # Service interfaces
│   ├── UserService.cs         # GitHub API interactions
│   └── ...
├── Models/                     # Data models
├── Endpoints/                  # API endpoints
└── wwwroot/                   # Static assets
```

## 🛠️ Technologies Used

### Backend

- **ASP.NET Core 9.0**: Web framework
- **Blazor Server**: Interactive web UI framework
- **C#**: Primary programming language

### Frontend

- **Blazor Components**: Interactive UI components
- **Bulma CSS**: Modern CSS framework
- **JavaScript**: Client-side interactions
- **Chart.js**: Data visualization

### Tools & Libraries

- **Markdig**: Markdown processing for README files
- **Memory Caching**: Performance optimization
- **HttpClient**: API communication
- **Docker**: Containerization

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. **Fork the repository**
2. **Create a feature branch**

   ```bash
   git checkout -b feature/AmazingFeature
   ```

3. **Commit your changes**

   ```bash
   git commit -m 'Add some AmazingFeature'
   ```

4. **Push to the branch**

   ```bash
   git push origin feature/AmazingFeature
   ```

5. **Open a Pull Request**

## 📝 API Usage

The application interacts with the GitHub API v3. Key endpoints used:

- `GET /users/{username}` - User profile information
- `GET /users/{username}/repos` - User repositories
- `GET /repos/{owner}/{repo}/readme` - Repository README files
- `GET /search/users` - User search functionality

## 🚨 Rate Limiting

- **Without Authentication**: 60 requests per hour
- **With Personal Access Token**: 5,000 requests per hour
- The application includes built-in rate limit monitoring and displays current usage

## 📊 Performance Features

- **Memory Caching**: Reduces API calls by caching responses
- **Lazy Loading**: Components load data as needed
- **Batch Processing**: Multiple API calls processed concurrently
- **Responsive Design**: Optimized for all device sizes

## 🔒 Security

- **No Server-Side Token Storage**: Tokens are stored only in browser local storage
- **HTTPS Enforced**: All communications encrypted in production
- **Input Validation**: All user inputs are validated and sanitized
- **Error Handling**: Secure error messages that don't leak sensitive information

## 📄 License

See the [LICENSE.txt](LICENSE.txt) file for details.

---
