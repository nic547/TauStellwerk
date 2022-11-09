import os
import shutil
from glob import glob

customargs="-c Release --self-contained /p:DebugSymbols=false /p:DebugType=None  /p:EnableCompressionInSingleFile=true /p:PublishTrimmed=true /p:TrimMode=partial /p:TreatWarningsAsErrors=false /nowarn:IL2026,IL2111,IL2104,IL2110,IL2091"
rids = ["linux-x64","linux-musl-x64","linux-arm64","linux-musl-arm64","win-x64","win-arm64"]

BLUE='\033[1;34m'
NC='\033[0m' # No Color

def main():
    
    shutil.rmtree("./publish", ignore_errors=True)
    delete_build_folders()
    os.system("dotnet clean")
    
    for rid in rids:
        print(f"{BLUE}Building {rid}{NC}")
        os.system(f"dotnet publish ./src/TauStellwerk.Server/ -r {rid} -o ./publish/{rid} {customargs}")
        remove_unneeded_files(rid)
        pack(rid)
    
def remove_unneeded_files(rid):
    os.remove(f"./publish/{rid}/appsettings.Development.json")
    if os.path.exists(f"./publish/{rid}/web.config"):
        os.remove(f"./publish/{rid}/web.config")
    shutil.rmtree(f"./publish/{rid}/BlazorDebugProxy")
    for file in glob(f"./publish/{rid}/*.xml"):
        os.remove(file)
        
        
def pack(rid):
    if rid.startswith("linux"):
        shutil.make_archive(f"./publish/TauStellwerk-{rid}", "gztar", f"./publish/{rid}")
    elif rid.startswith("win"):
        shutil.make_archive(f"./publish/TauStellwerk-{rid}", "zip", f"./publish/{rid}")
    else:
        raise Exception("Unkown platform")
    shutil.rmtree(f"./publish/{rid}")

def delete_build_folders():
    for folder in glob("./src/*/obj"):
        shutil.rmtree(folder)
    for folder in glob("./src/*/bin"):
        shutil.rmtree(folder)

main()