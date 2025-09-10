<<<<<<< HEAD
[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/dxXoyo55)
=======
# South African Municipal Services Portal

A comprehensive ASP.NET Core Razor Pages application for reporting municipal issues with gamification features, advanced data structures, and algorithms.

## 🏛️ Project Overview

This application allows citizens to report municipal issues, track their progress, earn badges, and compete on leaderboards. It demonstrates advanced C# programming concepts including multi-dimensional arrays, custom sorting algorithms, operator overloading, recursion, and advanced generics.

## ✨ Features

### Core Functionality
- **Issue Reporting**: Submit municipal issues with categories, priorities, and media attachments
- **User Management**: Registration, login, and session management
- **Badge System**: Gamification with achievement badges and points
- **Leaderboard**: Community competition and rankings
- **File Upload**: Support for image and video attachments
- **Responsive Design**: Works on desktop and mobile devices

### Advanced Features (Learning Units 1 & 2)
- **Multi-dimensional Arrays**: 2D/3D arrays for geographical data organization
- **Jagged Arrays**: Variable-length structures for municipality hierarchy
- **Custom Sorting Algorithms**: QuickSort, MergeSort, and HeapSort implementations
- **Array vs List Comparison**: Performance analysis and recommendations
- **Operator Overloading**: Custom operators for Issue comparison and operations
- **Recursion**: Recursive algorithms for data processing
- **Advanced Generics**: Generic repositories with type constraints
- **Geographical Analysis**: Complex data visualization using arrays

## 🚀 Getting Started

### Prerequisites
- .NET 9.0 SDK or later
- Visual Studio 2022 or VS Code
- Web browser (Chrome, Firefox, Safari, Edge)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd MunicapalServicesApp
   ```

2. **Navigate to project directory**
   ```bash
   cd MunicapalServicesApp
   ```

3. **Restore dependencies**
   ```bash
   dotnet restore
   ```

4. **Build the project**
   ```bash
   dotnet build
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

6. **Open in browser**
   Navigate to `https://localhost:5001` or `http://localhost:5000`

## 🔐 Demo Accounts

The application comes with pre-configured demo accounts for testing and demonstration:

### Primary Demo Account
- **Username**: `demo`
- **Password**: `demo123`
- **Email**: `demo@municipal.gov.za`
- **Full Name**: Demo User
- **Municipality**: Cape Town
- **Province**: Western Cape
- **Initial Points**: 150
- **Level**: 3
- **Reports Submitted**: 5
- **Badges Earned**: 3

### Additional Demo Accounts
- **Username**: `admin` | **Password**: `admin123` | **Email**: `admin@municipal.gov.za`
- **Username**: `testuser` | **Password**: `test123` | **Email**: `test@municipal.gov.za`
- **Username**: `citizen` | **Password**: `citizen123` | **Email**: `citizen@municipal.gov.za`

### Demo Account Features
- **Pre-loaded Data**: Each account has different levels of activity
- **Badge Progress**: Various badges already earned or in progress
- **Issue History**: Sample issues submitted for testing
- **Point System**: Different point levels to demonstrate progression
- **Session Management**: Persistent login sessions for testing

## 📁 Project Structure

```
MunicapalServicesApp/
├── Data/                          # Data access layer
│   ├── IssuesRepository.cs        # Issue data management
│   └── UserRepository.cs          # User data management
├── Models/                        # Data models
│   ├── Issue.cs                   # Issue model with operator overloading
│   ├── Badge.cs                   # Badge model
│   ├── User.cs                    # User model
│   └── UserSession.cs             # Session management
├── Pages/                         # Razor Pages
│   ├── Index.cshtml               # Home page
│   ├── ReportIssues.cshtml        # Issue reporting
│   ├── Badges.cshtml              # Badge collection
│   ├── Leaderboard.cshtml         # Community rankings
│   ├── AdvancedFeatures.cshtml    # Learning Units demo
│   └── Shared/
│       └── _Layout.cshtml         # Main layout
├── Services/                      # Business logic
│   ├── DataService.cs             # Main service layer
│   ├── BadgeService.cs            # Badge management
│   ├── MunicipalServiceManager.cs # Legacy service
│   └── AdvancedDataStructuresService.cs # Learning Units implementation
├── wwwroot/                       # Static files
│   ├── css/site.css               # Custom styling
│   ├── images/                    # Images and badges
│   └── uploads/                   # User uploaded files
└── Program.cs                     # Application entry point
```

## 🎯 Learning Units Implementation

### Learning Unit 1: Arrays and Sorting
- **Multi-dimensional Arrays**: `int[,] categoryLocationMatrix` for 2D data
- **3D Arrays**: `int[,,] priorityCategoryLocationMatrix` for complex mapping
- **Jagged Arrays**: `string[][] municipalityHierarchy` for variable-length data
- **Custom Sorting**: QuickSort, MergeSort, and HeapSort implementations
- **Performance Comparison**: Array vs List analysis and recommendations

### Learning Unit 2: Advanced C# Features
- **Operator Overloading**: Custom `>`, `<`, `+`, `*` operators for Issue class
- **Recursion**: Recursive counting, filtering, and calculation methods
- **Advanced Generics**: `Repository<T>` with type constraints
- **Generic Collections**: `SortedCollection<T>` with IComparable constraint
- **IComparable Implementation**: Custom sorting for Issue objects

## 🎮 Usage Guide

### Reporting Issues
1. Navigate to "Report Issue" from the main menu
2. Fill in issue details (title, location, category, description)
3. Set priority level (Low, Medium, High, Critical)
4. Upload media files (optional)
5. Submit the report

### Earning Badges
- **First Responder**: Submit your first issue
- **Community Helper**: Submit 3+ issues
- **Media Contributor**: Submit reports with attachments
- **Emergency Responder**: Submit critical priority issues
- **Category Specialist**: Submit 2+ issues in the same category
- **Consistent Reporter**: Submit 5+ issues
- **Community Champion**: Submit 10+ issues

### Advanced Features Demo
1. Navigate to "Advanced Features" from the main menu
2. Test custom sorting algorithms (QuickSort, MergeSort, HeapSort)
3. Compare Array vs List performance
4. Try operator overloading (issue comparison, merging, scaling)
5. Test recursive algorithms
6. Explore geographical data analysis

## 🔧 Technical Details

### Technologies Used
- **Framework**: ASP.NET Core 9.0
- **Language**: C# 12.0
- **UI**: Razor Pages with Bootstrap 5
- **Icons**: Font Awesome 6.0
- **Styling**: Custom CSS with South African theme

### Key Algorithms Implemented
- **QuickSort**: O(n log n) average case, O(n²) worst case
- **MergeSort**: O(n log n) guaranteed performance
- **HeapSort**: O(n log n) guaranteed performance
- **Recursive Counting**: O(n) time complexity
- **Recursive Filtering**: O(n) time complexity

### Data Structures Used
- **Lists**: `List<Issue>`, `List<Badge>`, `List<UserSession>`
- **Arrays**: Multi-dimensional and jagged arrays
- **Dictionaries**: `Dictionary<Guid, T>` for indexing
- **Generic Collections**: Custom generic repositories

## 🎨 Design Features

### South African Theme
- **Colors**: SA flag colors (green, gold, blue, red)
- **Typography**: Clean, readable fonts
- **Icons**: Font Awesome icons throughout
- **Layout**: Responsive design for all screen sizes

### User Experience
- **Intuitive Navigation**: Clear menu structure
- **Visual Feedback**: Success/error messages
- **Progress Tracking**: Badge progress indicators
- **Responsive Design**: Mobile-friendly interface

## 🐛 Troubleshooting

### Common Issues
1. **Port conflicts**: Change ports in `launchSettings.json`
2. **File upload errors**: Check `wwwroot/uploads` folder permissions
3. **Badge not displaying**: Ensure user email is set in issue submission
4. **Build errors**: Run `dotnet clean` then `dotnet build`

### Debug Mode
Run in debug mode for detailed error information:
```bash
dotnet run --environment Development
```

## 📊 Performance Considerations

### Optimizations Implemented
- **Efficient Sorting**: Custom algorithms for specific use cases
- **Memory Management**: Proper disposal of resources
- **Caching**: Static data caching where appropriate
- **Lazy Loading**: Data loaded only when needed

### Scalability
- **Repository Pattern**: Clean separation of concerns
- **Service Layer**: Business logic abstraction
- **Generic Implementations**: Reusable components
- **Async Operations**: Non-blocking file operations

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## 📝 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 👥 Authors

- **Your Name** - *Initial work* - [YourGitHub](https://github.com/yourusername)

## 🙏 Acknowledgments

- South African Municipal Services for inspiration
- ASP.NET Core team for the excellent framework
- Bootstrap team for the responsive design system
- Font Awesome for the comprehensive icon library

## 📞 Support

For support, email support@municipal.gov.za or create an issue in the repository.

---

**Note**: This application is designed for educational purposes and demonstrates advanced C# programming concepts as required by the learning units. The badge system and user engagement features are fully functional and ready for production use.
>>>>>>> 517ada6 (Student st10391223 Part 1 of Prog7312PoE)
