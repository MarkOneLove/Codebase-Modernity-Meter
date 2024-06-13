import matplotlib.pyplot as plt
import json
from datetime import datetime
import re
import os
import numpy as np
from pathlib import Path
from mpl_toolkits import mplot3d

# Specify the directory path
directory = Path('Results5')

# Initialize lists to store data
data = []

# Iterate over files in directory
for filename in os.listdir(directory):
    if filename.endswith('.json'):
        filepath = os.path.join(directory, filename)
        
        # Extract date from filename (assuming format: name_date.json)
        date_str = filename.split('_')[1].split('.')[0]  # Extract date part
        file_date = datetime.strptime(date_str, '%d-%m-%Y')  # Parse date string into datetime object
        
        # Read JSON file
        with open(filepath, 'r') as file:
            json_data = json.load(file)
            
            # Assuming json_data is a dictionary of {'7.1': 8, '7.2': 0, ...}
            data.append((file_date, json_data))

# Sort data by date
data.sort(key=lambda x: x[0])

# Extract unique versions
versions = list(data[0][1].keys())  # Extract versions from the first file (assuming all files have the same keys)

# Sort versions for consistent plotting order (smallest to biggest)
def version_key(version):
    parts = list(map(int, version.split('.')))
    return tuple(parts)

versions.sort(key=version_key)

# Prepare data for plotting
years = [date.year for date, _ in data]
months = [date.month for date, _ in data]
int_values = np.zeros((len(versions), len(data)), dtype=np.float64)  # Initialize matrix for integer values

for i, (_, json_data) in enumerate(data):
    for j, version in enumerate(versions):
        int_values[j, i] = json_data.get(version, 0)  # Fill in integer values, defaulting to 0 if version not present

# Max normalization
max_values = np.max(int_values, axis=1)  # Get maximum value for each version (across years)
normalized_values = int_values / max_values[:, np.newaxis]  # Divide each value by the respective maximum value

# Create meshgrid for plotting
X, Y = np.meshgrid(years, np.arange(len(versions)))

# Plotting
fig = plt.figure(figsize=(12, 8))
ax = fig.add_subplot(111, projection='3d')

# Rotate and adjust view angle
ax.view_init(elev=30, azim=135)

surf = ax.plot_surface(X, Y, normalized_values, cmap='Blues')

# Customize ticks and labels
ax.set_xticks(np.unique(years))
ax.set_yticks(np.arange(len(versions)))
ax.set_yticklabels(versions)
ax.set_xlabel('Year')
ax.set_ylabel('Version')
ax.set_zlabel('Normalized Values')
ax.set_title('Max Normalized Values across Years and Versions')

# Add color bar
fig.colorbar(surf, shrink=0.5, aspect=5)

plt.show()


# # List all files and directories in the specified directory
# contents = directory.iterdir()

# graphData = {}

# # Print the contents
# for item in contents:
#     with open(item, 'r') as file:

#         data = json.load(file)
#         year = re.split('-', str(item))[-1].replace(".json", "")
#         if year not in graphData:
#             graphData[year] = [data]
#         else:
#             graphData[year] += [data]
#         # Now 'data' contains the JSON data as a Python dictionary
        

# # print(graphData)

# # Extract data for plotting
# years = []
# versions = []
# int_values = []

# for year, versions_data in graphData.items():
#     for version_dict in versions_data:
#         for version_key, int_value in version_dict.items():
#             if version_key != 'year':  # Skip 'year' key if it exists
#                 years.append(int(year))
#                 versions.append(version_key)
#                 int_values.append(int_value)

# # Convert lists to numpy arrays
# years = np.array(years)
# versions = np.array(versions)
# int_values = np.array(int_values)

# # Extract unique versions and sort them
# versions_unique = np.unique(versions)
# versions_unique.sort()

# # Create a meshgrid for plotting
# years_unique = np.unique(years)
# X, Y = np.meshgrid(years_unique, np.arange(len(versions_unique)))  # Use np.arange for Y

# # Initialize Z values to zeros
# Z = np.zeros((len(versions_unique), len(years_unique)), dtype=np.float64)  # Ensure Z is float64

# # Populate Z values based on int_values
# for i, year in enumerate(years_unique):
#     for j, version in enumerate(versions_unique):
#         mask = (years == year) & (versions == version)
#         if np.any(mask):
#             Z[j, i] = int_values[mask][0]

# # Create the 3D plot
# fig = plt.figure(figsize=(12, 8))
# ax = fig.add_subplot(111, projection='3d')

# # Plot the surface with a custom colormap for blue shades
# surf = ax.plot_surface(X, Y, Z, cmap='Blues')

# # Customize ticks and labels
# ax.set_xticks(years_unique)
# ax.set_yticks(np.arange(len(versions_unique)))  # Set Y ticks based on number of unique versions
# ax.set_yticklabels(versions_unique)  # Set Y tick labels to the unique version names
# ax.set_xlabel('Year')
# ax.set_ylabel('Version')
# ax.set_zlabel('Integer Values')
# ax.set_title('Integer Values across Years and Versions')

# # Add a color bar which maps values to colors
# fig.colorbar(surf, shrink=0.5, aspect=5)

# # Show plot
# plt.show()