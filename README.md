# GitHub Profile Viewer

A modern, responsive web application built with Blazor Server that allows users to explore GitHub profiles and repositories with detailed insights, statistics, and visualizations. This comprehensive tool enables users to search for any GitHub user and view their profile details, repository information, star history, gists, and network connections all in one place.

[Live Demo](https://gpv.sametcc.me/)

## Authentication & Rate Limits

To increase the API rate limit and access private repository information, the application offers two authentication methods:

1. **Sign in with GitHub**: Securely sign in using your GitHub account.
2. **Personal Access Token**: Manually enter your GitHub Personal Access Token.

Both methods allow you to bypass the standard 60 requests/hour limit and utilize your own API quota (5,000 requests/hour).

## Features

### Search and Exploration

* **User Search**: Quickly search and find any GitHub user by their username. The real-time search feature provides instant results with user avatars and profile links.
* **Repository Explorer**: Browse through all public repositories with detailed information including stars, forks, language, and descriptions. Filter and sort repositories by various criteria.
* **Gist Viewer**: Access and browse user's gists with syntax highlighting. View code snippets, notes, and shared files directly from the profile page.
* **README Preview**: Read repository README files without leaving the application. Properly rendered markdown content helps you understand projects quickly.

### Analytics and Visualization

* **Star History Charts**: Visualize the growth of repository stars over time with interactive charts. Understand how a project's popularity has evolved since its creation.
* **Profile Statistics**: View comprehensive profile statistics including total stars received, total forks, primary languages, and contribution metrics.
* **Network Analysis**: Explore a user's network by viewing their followers and following lists. Identify connections and key contributors in the open-source community.

### API Documentation

The application includes a fully documented API powered by Scalar.
* **Interactive Docs**: Access the API documentation at `/docs` to explore endpoints and test requests.
* **Integration**: Use the API to programmatically access user profiles, repositories, and statistics.

### User Experience

* **Advanced Filtering**: Filter repositories by language, sort by stars or date, and search within a user's repositories to find exactly what you're looking for.
* **Responsive Design**: Fully responsive interface that works seamlessly across desktop, tablet, and mobile devices.
* **Rate Limit Monitoring**: Real-time display of API quota usage to help manage GitHub API limits.

## Technologies Used

### Backend

* **ASP.NET Core 9.0**: The core web framework used for the application.
* **Blazor Server**: Provides the interactive web UI framework.
* **C#**: The primary programming language for backend logic.

### Frontend

* **Blazor Components**: Reusable interactive UI components.
* **Bulma CSS**: A modern CSS framework used for styling and responsive design.
* **JavaScript**: Used for client-side interactions and integrations.
* **Chart.js**: Library used for rendering data visualization charts.

### Tools and Libraries

* **Markdig**: Used for processing and rendering Markdown content for README files.
* **Memory Caching**: Implemented to optimize performance and reduce API calls.
* **HttpClient**: Handles communication with the GitHub API.
* **Docker**: Support for containerized deployment.

## Getting Started

### Prerequisites

* .NET 9.0 SDK
* Git
* (Optional) Docker for containerized deployment

### Installation

1. Clone the repository:

    ```bash
    git clone https://github.com/sametcn99/GPVBlazor.git
    cd GPVBlazor
    ```

2. Navigate to the project directory:

    ```bash
    cd GPVBlazor/GPVBlazor
    ```

3. Restore dependencies:

    ```bash
    dotnet restore
    ```

4. Run the application:

    ```bash
    dotnet run
    ```

## Docker Deployment

To build and run the application using Docker:

1. Build the Docker image:

    ```bash
    docker build -t gpvblazor .
    ```

2. Run the container:

    ```bash
    docker run -d -p 8080:8080 --name gpvblazor-app gpvblazor
    ```

## Configuration

### GitHub Token Setup

The application interacts with the GitHub API. While it works without authentication, the API rate limit is restricted to 60 requests per hour. To increase this limit to 5,000 requests per hour and access private repository information, you can configure a Personal Access Token.

1. Generate a Personal Access Token in your GitHub Developer Settings.
2. Enter the token in the authentication section on the application's home page.
3. The token is stored securely in your browser's local storage and is never sent to any server other than GitHub's API.

## Project Structure

* **Components**: Contains all Blazor components (Displays, Layout, Pages).
* **Services**: Contains business logic and API interaction services (UserService, AuthService, etc.).
* **Models**: Defines data models used throughout the application.
* **wwwroot**: Contains static assets like CSS, JavaScript, and icons.

## License

See the LICENSE.txt file for details.
