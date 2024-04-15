
# !pip install opencv-python
# !pip install tensorflow
# !pip install wrap

import os
import cv2 as cv
from tensorflow.keras import models, layers, activations, \
optimizers, utils, losses, initializers, metrics, callbacks

folder_path = 'testing'
epochs = 100
batch_size = 32
patience = 5
learning_rate = 0.001
model_path = f'checkpoints/{folder_path}/test_model7.keras'
exists = os.path.exists(model_path)

# Carrega modelo se já existir um checkpoint, caso contrário, o cria.
if exists:
    model = models.load_model(model_path)
else: 
    model = models.Sequential([
        layers.Resizing(32, 32),
        layers.Rescaling(1.0/255),
        layers.BatchNormalization(axis=1),
        layers.RandomRotation((-0.05, 0.05)),
        layers.Conv2D(32, (3, 3),
            activation = 'relu',
            kernel_initializer = initializers.RandomNormal()
            ),
        layers.MaxPooling2D((2, 2)),
        layers.Conv2D(48, (3, 3),
            activation = 'relu',
            kernel_initializer = initializers.RandomNormal()
        ),
        # layers.MaxPooling2D((2, 2)), # tirar dps
        layers.Conv2D(32, (3, 3),
            activation = 'relu',
            kernel_initializer = initializers.RandomNormal()
        ),
        # layers.MaxPooling2D((2, 2)),
        layers.Flatten(),
        layers.Dropout(0.5),
        layers.Dense(128,
            activation = 'relu',
            kernel_initializer = initializers.RandomNormal()
        ),
        layers.Dense(80,
            activation = 'relu',
            kernel_initializer = initializers.RandomNormal()
        ),
        layers.Dense(62,
            activation = 'sigmoid',
            kernel_initializer = initializers.RandomNormal()
        )
        ])

if exists:
    model.summary()
else:
    model.compile(
    optimizer = optimizers.Adam(
    learning_rate = learning_rate
),
    loss = losses.SparseCategoricalCrossentropy(), # trocar 
    metrics = [ 'accuracy' ]
)
    
train = utils.image_dataset_from_directory(
    "organizedData",
    validation_split= 0.2,
    subset= "training",
    seed= 123,
    shuffle= True,
    image_size= (1200, 900),
    batch_size= batch_size
    )

test = utils.image_dataset_from_directory(
    "organizedData",
    validation_split= 0.2,
    subset= "validation",
    seed= 123,
    shuffle= True,
    image_size= (1200, 900),
    batch_size= batch_size
    )

model.fit(train,
    epochs = epochs,
    validation_data = test,
    callbacks= [
        callbacks.EarlyStopping(
            monitor = 'val_loss',
            patience = patience,
            verbose = 1
        ),
        callbacks.ModelCheckpoint(
            filepath = model_path,
            save_weights_only = False,
            monitor = 'loss',
            mode = 'min',
            save_best_only = True
        ),
        # callbacks.LearningRateScheduler(
        #     lambda epoch, lr : 1e-3 * 10 ** -(epoch / 20)
        # )
    ]
)


