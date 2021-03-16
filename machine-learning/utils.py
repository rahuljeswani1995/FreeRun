import numpy as np
import torch.nn as nn
import torch.nn.functional as F
from skimage.transform import resize



## Defining the neural network.
class Net(nn.Module):
    
    def __init__(self):
        super(Net, self).__init__()
        
        self.conv1 = nn.Conv2d(1, 4, kernel_size = 3, stride = 1, padding = 1)
        self.conv2 = nn.Conv2d(4, 4, kernel_size = 3, stride = 1, padding = 1)
        self.conv3 = nn.Conv2d(4, 4, kernel_size = 3, stride = 1, padding = 1)
        self.bn = nn.BatchNorm2d(4)
        self.mp = nn.MaxPool2d(kernel_size = 2, stride = 2)
        self.fc1 = nn.Linear(4096, 1024)
        self.fc2 = nn.Linear(1024, 512)
        self.fc3 = nn.Linear(512, 5)
        
        
    def forward(self, x):
        x = self.mp(F.relu(self.bn(self.conv1(x))))
        x = self.mp(F.relu(self.bn(self.conv2(x))))
        x = self.mp(F.relu(self.bn(self.conv3(x))))
        x = x.view(x.size(0), -1)
        x = F.relu(self.fc1(x))
        x = F.relu(self.fc2(x))
        x = self.fc3(x)
        return x

## Preprocess the image.
def preprocess(img, height, width):
    imgSize = (height, width)
    img = resize(np.array(img), imgSize)
    img = np.transpose(img)
    img = img.astype('float32')
    return img