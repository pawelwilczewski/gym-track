import { createApp } from 'vue';
import App from './App.vue';
import router from './Router';
import './assets/index.css';
import { createPinia } from 'pinia';

const app = createApp(App);

app.use(router);
app.use(createPinia());

app.mount('#app');
