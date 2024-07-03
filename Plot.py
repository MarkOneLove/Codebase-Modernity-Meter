import matplotlib.pyplot as plt
import json
from datetime import datetime
import re
import os
import numpy as np
import pandas as pd
from pathlib import Path
from mpl_toolkits import mplot3d
from matplotlib import cm
import matplotlib.dates as mdates

# Directory
data_directory = Path('Results')

# Data structs
dates = []
versions = []
feature_counts = []

# Extract date from name
def extract_date(filename):
    basename = os.path.basename(filename)
    date_str = os.path.splitext(basename)[0].split('_')[-1]  # extract date part from filename
    return datetime.strptime(date_str, '%d-%m-%Y')

# Iterate over each JSON file in the directory
for filename in os.listdir(data_directory):
    if filename.endswith('.json'):
        
        file_date = extract_date(filename)
        
        
        with open(os.path.join(data_directory, filename), 'r') as f:
            json_data = json.load(f)
        
        # Process JSON data
        for version, count in json_data.items():
            dates.append(file_date)
            versions.append(version)  # Store version as string to preserve decimal parts
            feature_counts.append(count)

# Convert dates to matplotlib date format
dates = mdates.date2num(dates)

# Convert lists to numpy arrays
dates = np.array(dates)
versions = np.array(versions)
feature_counts = np.array(feature_counts)

# Max normalization
max_feature_count = np.max(feature_counts)
if max_feature_count > 0:
    feature_counts_normalized = feature_counts / max_feature_count
else:
    feature_counts_normalized = feature_counts  # Handle case wher zero


unique_versions = np.unique(versions)
unique_versions_sorted = sorted(unique_versions, key=lambda v: float(v) if '.' in v else float(v + '.0'))

version_mapping = {v: i for i, v in enumerate(unique_versions_sorted)}

# Create 3D plot
fig = plt.figure(figsize=(10, 8))
ax = fig.add_subplot(111, projection='3d')

surf = ax.plot_trisurf(dates, [version_mapping[v] for v in versions], feature_counts_normalized, cmap='Blues', alpha=0.8)

ax.xaxis.set_major_formatter(mdates.DateFormatter('%Y-%m-%d'))

# Label axes and set title
ax.set_xlabel('Release date', labelpad=15)  
ax.set_ylabel('Language version')
ax.set_zlabel('Normalized amount of version-specific features')

ax.set_yticks(np.arange(len(unique_versions_sorted)))
ax.set_yticklabels(unique_versions_sorted)

# Rotate and align the tick labels so they look better
ax.set_yticklabels(ax.get_yticklabels(), rotation=45, ha='right')

# Show plot
plt.tight_layout()
plt.show()