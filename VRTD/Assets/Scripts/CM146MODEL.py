import numpy as np
import pandas as pd
from sklearn.decomposition import FastICA
from sklearn.model_selection import train_test_split
from sklearn.neighbors import KNeighborsClassifier
from sklearn.pipeline import make_pipeline, make_union
from sklearn.preprocessing import PolynomialFeatures
from tpot.builtins import StackingEstimator

# load dataset
url = "Downloads/Stress_classifier_with_AutoML_and_wearable_devices-master/dataset/dataframe_hrv.csv"
dataframe = pd.read_csv(url)
dataframe = dataframe.reset_index(drop=True)
# split into input and output elements
# data = dataframe.values
# data = data.astype('float32')
# print(dataframe)

def labelling_stress(df='', label_column='stress'):
    df['stress'] = np.where(df['stress']>=0.5, 1, 0)
    display(df["stress"].unique())
    return df

def fill_mean_values(df):
    df = df.reset_index()
    df = df.replace([np.inf, -np.inf], np.nan)
    
    df[~np.isfinite(df)] = np.nan
    # print(df.head(5))
    # df.plot( y=["HR"])
    df['HR'].fillna((df['HR'].mean()), inplace=True)
    # df['TP'].fillna((df['TP'].mean()), inplace=True)
    # print(df.head(5))
    # print("\n",df.head(5))
    
    #noise reduction
    df['HR'] = signal.medfilt(df['HR'],13) 
    # print(df.head(5))
    # df.plot( y=["HR"])

    df.fillna(df.mean(),inplace=True)
    
    return df

dataframe = labelling_stress(df=dataframe)
# print(dataset)
dataframe = fill_mean_values(dataframe)

# dataframe = dataframe.to_numpy()
# print(dataframe)
# X, y = dataframe['HR'].reshape(-1, 1), dataframe['stress']
X = dataframe['HR']
# print(X)
X = X.to_numpy().reshape(-1, 1)
# X = dataframe['HR']
y = dataframe['stress']
y = y.to_numpy()
print(y)



# Average CV score on the training set was: 0.785967365967366
exported_pipeline = make_pipeline(
    make_union(
        FastICA(tol=0.1),
        make_pipeline(
            FastICA(tol=0.1),
            PolynomialFeatures(degree=2, include_bias=False, interaction_only=False)
        )
    ),
    FastICA(tol=0.35000000000000003),
    KNeighborsClassifier(n_neighbors=85, p=1, weights="distance")
)

# Fix random state in exported estimator
if hasattr(exported_pipeline, 'random_state'):
    setattr(exported_pipeline, 'random_state', 1)
# fit the model
exported_pipeline.fit(X, y)


while True:
    player_data = pd.read_csv("Downloads/Stress_classifier_with_AutoML_and_wearable_devices-master/sampleHR.csv")
    # make a prediction on a new row of data
    testing_features = player_data[-1:].to_numpy()
    # print("old test:",testing_features)
    last_data = player_data.iloc[-1]['HR']
    print(player_data)
    # print("%s: %f", a, last_data)

     
    
    if last_data == -1:
      temp = player_data['HR'].iloc[[-2,-3,-4,-5]].mean(axis=0)
      player_data['HR'] = player_data['HR'].replace(-1, temp)
      player_data.to_csv("Downloads/Stress_classifier_with_AutoML_and_wearable_devices-master/sampleHR.csv", index=False)
      player_data = pd.read_csv("Downloads/Stress_classifier_with_AutoML_and_wearable_devices-master/sampleHR.csv")
      # print(player_data)
      testing_features = player_data[-1:].to_numpy()
      # print("new test:",testing_features)
      prediction = exported_pipeline.predict(testing_features)
      print('Predicted: %.3f' % prediction[0])
    else:
      prediction = exported_pipeline.predict(testing_features)
      print('Predicted: %.3f' % prediction[0])