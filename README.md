# RG35XX.NET

A brief overview of RG35XX.NET while waiting for full documentation.

## Build Configurations

- **Release Configuration**: Builds the application for handheld devices.
- **Debug Configuration**: Builds the application for desktop.

## Primary Classes

### 1. ConsoleRenderer
Provides an easy method for writing text to the `FrameBuffer`. Console letters are mapped to bitmaps and rendered as they would appear on a console.

---

### 2. StorageProvider
Abstracts storage locations based on the platform:
- **Desktop**: 
  - `ROOT` maps to the application directory.
  - `MMC` and `SDCARD` map to subdirectories within the application directory.
- **Handheld**:
  - `ROOT` maps to the device root.
  - `MMC` and `SDCARD` map to the mount points for each card.

---

### 3. GamePadReader
Provides both blocking and non-blocking methods to read input from the gamepad.

#### Desktop Keyboard Mappings:
- **Enter**: Start
- **Escape**: Menu
- **A**: A
- **B**: B
- **Y**: Y
- **X**: X
- **, (<)**: L1
- **. (>)**: L2
- **Arrow Keys**: DPad

Additional mappings may be added over time for desktop.

---

### 4. FrameBuffer
Provides an interface for writing to the output device:
- **Handheld**: The screen.
- **Desktop**: The window.

#### FrameBuffer Details:
- Accepts a custom "Bitmap" structure, which is an object backed by ARGB pixels.
- Includes basic drawing operations.
- Optimized for mapping images directly from external sources or embedded content.
- Runtime drawing is supported but should ideally be minimized. Drawing functionality can be extended as needed.

---

## Setup Instructions

1. Include the project files as references in your application.
2. Reference only the **Libraries** project; all dependencies will be handled automatically.

---

## Publishing

- Use the included `FolderProfile.pubxml` for publishing, or use it as a reference for custom settings.
