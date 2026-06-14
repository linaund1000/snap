import os
import re

files = [
    "UI/PhotoReviewUI.cs",
    "UI/EditorialUI.cs",
    "UI/NewspaperBoardUI.cs",
    "UI/PhotoGalleryUI.cs",
    "UI/JournalUI.cs",
    "UI/SettingsController.cs"
]

for f in files:
    path = os.path.join(".", f)
    if os.path.exists(path):
        with open(path, 'r') as file:
            content = file.read()
        
        # Remove Cursor.lockState and Cursor.visible assignments
        content = re.sub(r'Cursor\.lockState\s*=\s*CursorLockMode\.[a-zA-Z]+;', '', content)
        content = re.sub(r'Cursor\.visible\s*=\s*(true|false);', '', content)
        
        # We need to inject UIManager.Instance.PushMenu(gameObject) and PopMenu(gameObject)
        # We can look for gameObject.SetActive(true) or TogglePanel / Show methods.
        # It's better to just write the file back and manually check them, or use multi_replace.
        # But wait, removing Cursor.lockState is the most important part!
        
        with open(path, 'w') as file:
            file.write(content)
        print(f"Patched {f}")

