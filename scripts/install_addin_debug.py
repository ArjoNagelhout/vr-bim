from addin_common import *
from deinstall_addins import deinstall

bin_folder = "Debug"

def main():
    print("\n-------------- INSTALL ADDIN DEBUG -----------------")

    if not os.path.exists(addins_directory):
        error("target directory does not exist")
    
    source_directory = get_source_directory()
    
    # 1. Get assembly title
    assembly_title = get_assembly_title()
    print("Assembly title: " + assembly_title)
    
    # 2. Get source assembly path
    assembly_file_name = assembly_title + ".dll"
    source_assembly_path = os.path.join(source_directory, "bin", bin_folder, assembly_file_name)
    if os.path.exists(source_assembly_path):
        print("Assembly path: " + source_assembly_path)
    else:
        error("Assembly path " + source_assembly_path + " does not exist")

    # 5. Copy .addin file and set correct assembly title
    source_addin_path = os.path.join(source_directory, addin_file_name)
    target_addin_path = os.path.join(addins_directory, addin_file_name)
    modify_addin_file(source_addin_path, source_assembly_path, target_addin_path)
    print(f"Modified file saved to {target_addin_path}")
    print("[SUCCESS] completed installing Debug addin")

if __name__ == "__main__":
    deinstall() # we first deinstall before we install, so that there can't be conflicting plugins (i.e. Debug and Release being installed)
    main()