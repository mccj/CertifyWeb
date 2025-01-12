<template>
  <div>
    <el-descriptions title="证书颁发机构" width="100%" style="width: 100%" :column="5">
      <template v-for="item in state.tableData">
        <el-descriptions-item label="名称">{{item.title}}</el-descriptions-item>
        <el-descriptions-item label="网址">
          <el-link :href="item.websiteUrl" target="_blank">{{item.websiteUrl}}</el-link>
        </el-descriptions-item>
        <el-descriptions-item label="签发有效期">{{item.standardExpiryDays}} 天</el-descriptions-item>
        <el-descriptions-item label="签名功能">
          <template v-for="item2 in item.supportedFeatures">
            <el-tag type="info" v-if="item2=='DOMAIN_SINGLE'">单域名</el-tag>
            <el-tag type="info" v-else-if="item2=='DOMAIN_MULTIPLE_SAN'">多域名</el-tag>
            <el-tag type="info" v-else-if="item2=='DOMAIN_WILDCARD'">通配符域名</el-tag>
            <el-tag type="info" v-else>{{item2}}</el-tag>
          </template>
        </el-descriptions-item>
        <el-descriptions-item label="支持密钥算法">
          <el-tag type="info" v-for="item2 in item.supportedKeyTypes">{{item2}}</el-tag>
        </el-descriptions-item>
        <el-descriptions-item label="描述" span="5">{{item.description}}</el-descriptions-item>
        <el-descriptions-item label="eab说明" span="5">{{item.eabInstructions}}</el-descriptions-item>
      </template>
    </el-descriptions>
    <!--<br>
    <el-pagination :page-sizes="[10, 20, 30, 50, 100, 200]" :disabled="false" :background="true"
      layout="total, sizes, prev, pager, next, jumper" :total="state.tableData?.total ?? 0"
      v-model:page-size="state.pagination.pageSize" v-model:current-page="state.pagination.pageNo"
      @change="loadDatas" />-->
  </div>
</template>

<script setup lang="ts">
  import { reactive, ref, onMounted } from 'vue';
  //import moment from 'moment';
  //import { formatDate, fromNow } from '@/utils/format-handle';
  import client, { CertificateAuthority } from '@/api';

  const state = reactive({
    tableData: [] as CertificateAuthority[],
    isLoading: false,
    //query: {} as HouseholdGetPageRequestType,
    //pagination: {
    //  pageSize: 10,
    //  pageNo: 1
    //}
  });

  onMounted(async () => {
    await loadDatas();
  });

  const loadDatas = async () => {
    state.isLoading = true;
    state.tableData = await client.ApiAcmeCertificateGetCertificateAuthorities(/*Object.assign({}, state.query, { pageNo: state.pagination.pageNo, pageSize: state.pagination.pageSize })*/);
    state.isLoading = false;
  };

</script>

<style lang="scss" scoped></style>
