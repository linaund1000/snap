import os

files = [
    "SearchAction.cs", "ApproachPlayerAction.cs", "ReadNewsAction.cs", "ChillAloneAction.cs", 
    "WanderAction.cs", "CryInstinctAction.cs", "SocializeAction.cs", "GroupHangoutAction.cs", 
    "GroupWalkAction.cs", "FleePlayerAction.cs", "GossipAction.cs", "GoHomeAction.cs", 
    "ArgueAction.cs", "FleeAction.cs", "TravelAction.cs"
]

for f in files:
    if os.path.exists(f):
        with open(f, 'r') as file:
            lines = file.readlines()
        
        out = []
        skip = False
        braces = 0
        for line in lines:
            if "public override float CalculateUtility()" in line:
                skip = True
                braces = 0
            
            if skip:
                braces += line.count('{')
                braces -= line.count('}')
                if braces == 0 and '{' in line or '}' in line: # if it just matched public override line which doesn't have brace, brace is 0
                    if braces == 0 and '}' in line:
                        skip = False
                continue
            
            out.append(line)
            
        with open(f, 'w') as file:
            file.writelines(out)
        print(f"Processed {f}")

