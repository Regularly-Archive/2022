import matplotlib.pyplot as plt
import numpy as np
from matplotlib import cm
from mpl_toolkits.mplot3d import Axes3D

fig = plt.figure()
ax = fig.gca(projection='3d')

n_radii = 100 # 平滑度
n_angles = 100 # 鞍部半径

radii = np.linspace(0.125, 1.0, n_radii)
angles = np.linspace(0, 2*np.pi, n_angles, endpoint=False)
angles = np.repeat(angles[..., np.newaxis], n_radii, axis=1)

x = np.append(0, (radii*np.cos(angles)).flatten())
y = np.append(0, (radii*np.sin(angles)).flatten())
# z = np.sin(-x*y)
z = (np.power(y, 2) / 6) - (np.power(x, 2) / 4)

ax.plot_trisurf(x, y, z, linewidth=0.2, cmap=cm.jet, antialiased=True)

plt.show()
