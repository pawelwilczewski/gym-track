const setDark: () => void = () => {
  document.documentElement.classList.add('dark');
};

const setLight: () => void = () => {
  document.documentElement.classList.remove('dark');
};

if (window.matchMedia('(prefers-color-scheme: dark)').matches) {
  setDark();
} else {
  setLight();
}

window
  .matchMedia('(prefers-color-scheme: dark)')
  .addEventListener('change', (e) => {
    if (e.matches) {
      setDark();
    } else {
      setLight();
    }
  });
