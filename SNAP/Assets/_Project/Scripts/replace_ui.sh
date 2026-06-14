#!/bin/bash
for f in UI/PhotoReviewUI.cs UI/NewspaperBoardUI.cs UI/PhotoGalleryUI.cs UI/JournalUI.cs UI/SettingsController.cs; do
    sed -i '' 's/if (GPOyun.Core.ServiceLocator.Get<GPOyun.Core.GameManager>() != null) GPOyun.Core.ServiceLocator.Get<GPOyun.Core.GameManager>().PauseGame();/GPOyun.UI.UIManager.Instance?.PushMenu(gameObject);/g' "$f"
    sed -i '' 's/if (GPOyun.Core.ServiceLocator.Get<GPOyun.Core.GameManager>() != null) GPOyun.Core.ServiceLocator.Get<GPOyun.Core.GameManager>().ResumeGame();/GPOyun.UI.UIManager.Instance?.PopMenu(gameObject);/g' "$f"
done

sed -i '' 's/if (GPOyun.Core.ServiceLocator.TryGet<GPOyun.Core.GameManager>(out var gmPause)) gmPause.PauseGame();/GPOyun.UI.UIManager.Instance?.PushMenu(gameObject);/g' UI/EditorialUI.cs
sed -i '' 's/if (GPOyun.Core.ServiceLocator.TryGet<GPOyun.Core.GameManager>(out var gmResume)) gmResume.ResumeGame();/GPOyun.UI.UIManager.Instance?.PopMenu(gameObject);/g' UI/EditorialUI.cs
