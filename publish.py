from __future__ import print_function

import os

def c(s):
    print(">",s)
    os.system(s)

os.chdir("PickleShareNet")
c("dotnet pack -c Release")