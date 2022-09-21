<template>
  <div id="app">
    <img alt="Vue logo" src="./assets/logo.png"  width="150px" height="150px"/>
    <header>
      <router-link :to="{ path: 'home' }">首页</router-link>
      <span> - </span>
      <router-link :to="{ path: 'iframe', query: { url: 'https://www.bing.com' }}">Bing</router-link>
      <span> - </span>
      <router-link :to="{ path: 'iframe', query: { url: 'https://blog.yuanpei.me' }}">Blog</router-link>
    </header>
    <hr/>
    <router-view />
  </div>
</template>

<script>

export default {
  name: 'App',
  components: {
  }
}
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>
