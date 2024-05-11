from addin_common import *

def deinstall():
    print("\n-------------- DEINSTALL ADDIN (DEBUG AND RELEASE) -----------------")

    if not os.path.exists(addins_directory):
        error("target directory does not exist")
    
    source_directory = get_source_directory()
    
    # 1. Get assembly title
    assembly_title = get_assembly_title()
    print("Assembly title: " + assembly_title)

    # 2. deinstall debug (just the file)
    target_addin_path = os.path.join(addins_directory, addin_file_name)
    if os.path.exists(target_addin_path):
        os.remove(target_addin_path)
        print("[SUCCESS] Removed debug addin file " + target_addin_path)
    else:
        print("[SUCCESS] No debug addin found")
        
    
    # 3. deinstall release
    target_directory = os.path.join(addins_directory, assembly_title)
    if os.path.exists(target_directory):
        shutil.rmtree(target_directory)
        print("[SUCCESS] Removed release addin directory: " + target_directory)
    else:
        print("[SUCCESS] No release addin found")


if __name__ == "__main__":
    deinstall()