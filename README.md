InkDrawvsStorkeContainer ,
BoundRect issue when saving to png. 
This is to report an issue which might be a bug with the InkPresenter.StrokeContainer.

Clone/Download the UWP app (Anniversary build in my case) in VS (2017 community)
Run the app. 
You will see a button to load a gif file. Load the gif file provided, TestFile.gif . 
Click Save, and it will save it to PNG. 
Observe the boundaries on the top and bottom and how the image is clipped, in disagreement with the original strokes seen on the InkCanvas.
Discussion is here:
https://github.com/Microsoft/Win2D/issues/505
