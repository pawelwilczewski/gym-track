import { createApp } from 'vue';
import App from './App.vue';
import router from './router';
import './assets/index.css';
import '@/scripts/Math/RoundingExtensions';
import '@/scripts/Styling/AutoUpdateBodyDarkClass';

createApp(App).use(router).mount('#app');
