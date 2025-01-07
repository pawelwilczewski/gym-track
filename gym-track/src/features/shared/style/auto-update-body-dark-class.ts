const setDark: () => void = () => {
  document.documentElement.classList.add('dark');
};

const setLight: () => void = () => {
  document.documentElement.classList.remove('dark');
};

if (globalThis.matchMedia('(prefers-color-scheme: dark)').matches) {
  setDark();
} else {
  setLight();
}

globalThis
  .matchMedia('(prefers-color-scheme: dark)')
  .addEventListener('change', event => {
    if (event.matches) {
      setDark();
    } else {
      setLight();
    }
  });
