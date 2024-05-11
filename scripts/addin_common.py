import os
import sys

import shutil

addins_directory = r'C:\Program Files\Autodesk\Revit 2024\AddIns'
revit_plugin_directory_name = "revit-to-vr-plugin"
addin_file_name = "RevitToVRPlugin.addin"

def error(message):
    print("[ERROR] " + message)
    exit(1)

def get_source_directory():
    source_directory = os.path.join(os.path.dirname(os.path.dirname(os.path.abspath(__file__))), revit_plugin_directory_name)
    if not os.path.exists(source_directory):
        error("source directory does not exist")
        return 
    return source_directory

def get_assembly_title():
    source_directory = get_source_directory()
    assembly_info_path = os.path.join(source_directory, 'Properties', 'AssemblyInfo.cs')
    assembly_title = get_assembly_title_internal(assembly_info_path)
    if not assembly_title:
        error("Assembly title could not be found.")
    return assembly_title

def get_assembly_title_internal(assembly_info_path):
    # Extract the assembly title from the specified file
    title_prefix = '[assembly: AssemblyTitle("'
    with open(assembly_info_path, 'r') as file:
        for line in file:
            if line.strip().startswith(title_prefix):
                # Extract title between the quotes
                start_index = line.find('"') + 1
                end_index = line.find('"', start_index)
                return line[start_index:end_index]
    return None

def modify_addin_file(source_file_path, assembly_path, target_file_path):
    # Read the content from the source file
    with open(source_file_path, 'r') as file:
        content = file.read()

    # Replace {AssemblyName} with the new assembly name
    modified_content = content.replace('{AssemblyName}', assembly_path)

    # Write the modified content to the new location
    with open(target_file_path, 'w') as file:
        file.write(modified_content)

def prepare_target_directory(directory_path):
    # if the target directory doesn't exist, create it
    if not os.path.exists(directory_path):
        os.makedirs(directory_path)
        return  # success

    if not os.path.isdir(directory_path):
        error(directory_path + " is an existing file, not a directory")

    # directory exists already, see if it is empty
    items = os.listdir(directory_path)

    if len(items) > 0:
        print("Directory \"" + directory_path + "\" is not empty, containing the following files:")
        print(items)
        answer = input("Do you want to empty the folder? [Y/N]")
        answer = answer.lower()
        if answer.startswith('y') or answer == "":
            shutil.rmtree(directory_path)
            os.makedirs(directory_path)
        else:
            error("did not empty folder")