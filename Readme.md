# MyMLP - An Interactive Neural Network Playground

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![.NET Version](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
[![Platform](https://img.shields.io/badge/Platform-Windows-informational.svg)](https://www.microsoft.com/en-us/windows)

**MyMLP** is a powerful and interactive desktop application for visualizing and training Multi-Layer Perceptron (MLP) neural networks. Built with **.NET 8** and **WPF**, this tool provides an immersive, hands-on environment for students, developers, and researchers to explore the fundamentals of neural networks, understand their architecture, and observe the fascinating process of machine learning in real-time.

---

### Dark Theme Showcase
![Dark Theme UI](https://github.com/amirrezamortazavifard/MyMLP/blob/main/images/DarkTheme.png)

### Light Theme Showcase
![Light Theme UI](https://github.com/amirrezamortazavifard/MyMLP/blob/main/images/LightTheme.png)

---

## ✨ Core Features

- **Dynamic & Interactive Network Visualization**: Construct and visualize neural network architectures on the fly. Watch as weights and connections evolve, thicken, and change color during training, providing intuitive feedback on the learning process.

- **Real-Time Performance Dashboard**: Monitor key metrics like **Mean Squared Error (MSE)** and **Accuracy** on live-updating charts, powered by the high-performance LiveCharts library.

- **Extensive Built-in Datasets**: Instantly generate 8 classic and challenging datasets, including Circles, XOR, Spirals, Two Moons, Gaussian clusters, and more. Interactively control sample size and noise levels to see their immediate effect on the data topology.

- **Custom Data Support**: Go beyond the built-ins. Load your own datasets from `.csv` files to train and test the network on your specific problems.

- **Fully Configurable Architecture**: Easily configure the number of input, output, and hidden layer neurons. Experiment with different activation functions (**Sigmoid, ReLU, Tanh**) to understand their impact on learning.

- **Advanced Training Control**: Seize full control over the training process with adjustable **Learning Rate** and **Epochs**. A dedicated visualization speed slider lets you slow down or speed up the UI updates. **Play, Pause, and Reset** the training at any moment.

- **Model Persistence**: Don't lose your progress. Save your trained neural network models to a versioned `.mlpjson` file and load them back later to continue training or for further analysis.

- **One-Click Session Export**: A professional reporting tool to export the entire session state with a single click. This feature saves high-quality PNG images of the **Data View**, **Network Architecture**, **Performance Charts**, and a full text file of the **Training Log**.

- **Stunning, Themed UI**: A meticulously crafted, fully-custom user interface with seamless switching between **Dark** and **Light** themes. The custom-drawn window chrome (title bar, buttons) and themed controls (scrollbars, status bar) provide a completely immersive and branded experience.

---

## 🛠️ Architecture & Technology Stack

This project is a showcase of modern desktop development practices using the .NET ecosystem, with a strong emphasis on clean architecture and maintainability.

- **Framework**: **.NET 8**
- **UI Framework**: **WPF (Windows Presentation Foundation)**
- **Core Architecture**: **MVVM (Model-View-ViewModel)**
  - **Views**: Pure XAML with minimal, streamlined code-behind, focused solely on UI events.
  - **ViewModels**: Manages all application logic and state, completely decoupled from the UI.
  - **Models & Core**: Encapsulates the neural network logic, data structures, and business rules.
  - **Services**: Abstracted logic for data loading, model persistence, data generation, and UI snapshotting, promoting the Dependency Inversion Principle.
- **Key Libraries**:
  - **[LiveCharts.Wpf](https://lvcharts.net/wpf)**: For beautiful, responsive, and high-performance real-time charting.
- **Key Engineering Principles**:
  - **Separation of Concerns (SoC)**: A clear distinction between UI, application logic, and core engine.
  - **Asynchronous Programming (`async`/`await`)**: Ensures a responsive, non-blocking UI during intensive operations like training and rendering.
  - **Advanced XAML**: Heavy use of DataBinding, Converters, `ControlTemplate`s, and `DynamicResource`s for a flexible and maintainable UI.

---

## 🚀 Getting Started

### Prerequisites

- **Visual Studio 2022** (or later) with the ".NET desktop development" workload installed.
- **.NET 8 SDK**.

### Installation & Running

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/amirrezamortazavifard/MyMLP.git
    ```
2.  **Navigate to the project directory:**
    ```bash
    cd MyMLP
    ```
3.  **Open the solution:**
    Open `MyMLP.sln` in Visual Studio.
4.  **Restore dependencies:**
    The NuGet packages (like LiveCharts) should restore automatically upon opening the solution. If not, right-click the solution in Solution Explorer and select "Restore NuGet Packages".
5.  **Run the application:**
    Press `F5` or click the "Start" button to build and run the project in Debug mode.

---

## 📖 How to Use the Playground

1.  **Configure the Network**: In the "Network Configuration" panel, define the structure of your MLP. For example, `4,8,2` creates two hidden layers with 4 and 8 neurons, respectively.
2.  **Provide Data**:
    - **Generate**: Select one of the 8 built-in dataset types. Use the sliders to adjust the sample count and noise level and watch the data view update in real-time.
    - **Load**: Click "Load CSV Data File" to import your own dataset. The file should be formatted with features first, followed by one-hot encoded expected outputs (e.g., for a 2-feature, 2-class problem: `feature1,feature2,output1,output2`).
3.  **Configure Training**: Set the desired **Learning Rate** and the maximum number of **Epochs**.
4.  **Start Training**:
    - Click **▶ Play** to begin the training process.
    - Observe the charts, network visualization, and logs update in real-time.
    - Use **❚❚ Pause** to halt the training and **■ Reset** to re-initialize the network with new random weights.
5.  **Export Your Results**:
    - Once training is complete or paused, click **Export Session**.
    - Choose a base file name and location. The application will automatically save a complete report of your experiment (4 files in total).

---

## 🤝 Contributing

Contributions, issues, and feature requests are welcome! Feel free to check the [issues page](https://github.com/amirrezamortazavifard/MyMLP/issues). If you have an idea for a new feature or dataset, please open an issue first to discuss it.

To contribute:
1. Fork the Project.
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`).
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`).
4. Push to the Branch (`git push origin feature/AmazingFeature`).
5. Open a Pull Request.

## 📄 License

This project is distributed under the MIT License. See `LICENSE` for more information.
