<template>
  <div>
    <iframe id="vue-iframe" :src="url" frameborder="0" scrolling="auto" height="500px" width="1920px"/>
  </div>
</template>

<script>
export default {
  name: 'VueIframe',
  computed: {
    url() {
        return this.$route.query.url
    }
  }
}
</script>

