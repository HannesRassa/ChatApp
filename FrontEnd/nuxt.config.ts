// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2024-04-03',
  devtools: { enabled: true },
  modules: ['@nuxt/ui', '@pinia/nuxt'],
  
  runtimeConfig:{
    public:{exercisesApiUrl: "https://localhost:7269/api/"},
  },
})