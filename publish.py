import os
import shutil
from glob import glob

customargs="-c Release /p:DebugSymbols=false /p:DebugType=None"
rids = ["linux-x64","linux-musl-x64","linux-arm64","linux-musl-arm64","win-x64","win-arm64"]

BLUE='\033[1;34m'
NC='\033[0m' # No Color

def main():
    
    shutil.rmtree("./publish", ignore_errors=True)
    delete_build_folders()
    os.system("dotnet clean")
    
    for rid in rids:
        print(f"{BLUE}Building Server {rid}{NC}")
        os.system(f"dotnet publish ./src/TauStellwerk.Server/ -r {rid} -o ./publish/TauStellwerk.Server-{rid} {customargs}")
        remove_unneeded_files(rid)
        pack(rid, "Server")

    for rid in rids:
        print(f"{BLUE}Building Desktop {rid}{NC}")
        os.system(f"dotnet publish ./src/TauStellwerk.Desktop/ -r {rid} -o ./publish/TauStellwerk.Desktop-{rid}")
        pack(rid, "Desktop")
    
def remove_unneeded_files(rid):
    if os.path.exists(f"./publish/TauStellwerk.Server-{rid}/appsettings.Development.json"):
        os.remove(f"./publish/TauStellwerk.Server-{rid}/appsettings.Development.json")
    if os.path.exists(f"./publish/TauStellwerk.Server-{rid}/web.config"):
        os.remove(f"./publish/TauStellwerk.Server-{rid}/web.config")
    shutil.rmtree(f"./publish/TauStellwerk.Server-{rid}/BlazorDebugProxy")
    for file in glob(f"./publish/TauStellwerk.Server-{rid}/*.xml"):
        os.remove(file)
        
        
def pack(rid,type):
    if rid.startswith("linux"):
        shutil.make_archive(f"./publish/TauStellwerk.{type}-{rid}", "gztar", f"./publish/TauStellwerk.{type}-{rid}")
    elif rid.startswith("win"):
        shutil.make_archive(f"./publish/TauStellwerk.{type}-{rid}", "zip", f"./publish/TauStellwerk.{type}-{rid}")
    else:
        raise Exception("Unkown platform")
    shutil.rmtree(f"./publish/TauStellwerk.{type}-{rid}")

def delete_build_folders():
    for folder in glob("./src/*/obj"):
        shutil.rmtree(folder)
    for folder in glob("./src/*/bin"):
        shutil.rmtree(folder)

main()