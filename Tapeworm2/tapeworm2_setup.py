import sys
import os

def setup_tapeworm_path():
    project_dir = r"C:\Users\Antoine\source\repos\Tapeworm2"
    tapeworm_dir = os.path.join(project_dir, "src", "tapeworm")
    if tapeworm_dir not in sys.path:
        sys.path.append(tapeworm_dir)
    print("Updated sys.path:", sys.path)  # Add this line

# Add this line at the end of the file
setup_tapeworm_path()